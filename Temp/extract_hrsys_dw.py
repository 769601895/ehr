# -*- coding: utf-8 -*-
import re
from pathlib import Path

path = Path(r"E:\ehr2026\ehr\yuenan\hrs.pbl")
data = path.read_bytes()

def find_all(enc_name, needle):
    b = needle.encode(enc_name) if isinstance(needle, str) else needle
    out = []
    start = 0
    while True:
        i = data.find(b, start)
        if i < 0:
            break
        out.append(i)
        start = i + 1
    return out

for label, needle in [
    ("u16 d_rs_ddw_sysoptions", "d_rs_ddw_sysoptions"),
    ("ascii hr_sys_mstr", b"hr_sys_mstr"),
    ("u16 hr_sys_mstr", "hr_sys_mstr".encode("utf-16le")),
    ("gbk 人事参数", "人事参数设置"),
    ("u16 人事参数", "人事参数设置".encode("utf-16le")),
    ("gbk values", b'values="'),
    ("ascii sys_emp_id", b"sys_emp_id"),
    ("u16 sys_emp_id", "sys_emp_id".encode("utf-16le")),
]:
    if isinstance(needle, str):
        idxs = find_all("utf-16le", needle) if label.startswith("u16") and not isinstance(needle, bytes) else find_all("gbk", needle)
        if label.startswith("u16") and isinstance(needle, str):
            idxs = find_all("utf-16le", needle)
        elif label.startswith("gbk") and isinstance(needle, str):
            idxs = find_all("gbk", needle)
        else:
            idxs = []
            start = 0
            b = needle if isinstance(needle, bytes) else needle.encode()
            while True:
                i = data.find(b, start)
                if i < 0:
                    break
                idxs.append(i)
                start = i + 1
    else:
        idxs = []
        start = 0
        while True:
            i = data.find(needle, start)
            if i < 0:
                break
            idxs.append(i)
            start = i + 1
    print(label, "count", len(idxs), "first", idxs[:5])

# Extract a large UTF-16LE window around first d_rs_ddw_sysoptions
idx = data.find("d_rs_ddw_sysoptions".encode("utf-16le"))
chunk = data[max(0, idx - 200) : idx + 200000]
# decode as utf-16le ignoring errors, keep printable-ish
text = chunk.decode("utf-16le", errors="ignore")
# Also try decoding same region as gbk for interleaved ANSI source
# PowerBuilder often stores source as single-byte with GBK Chinese
# Search for ANSI source near object: look for 'release' after name as ANSI
idx_a = data.find(b"release 12")
print("release12 ascii", idx_a)

# Find all column values= patterns in utf16 text near sysoptions
out_path = Path(r"E:\ehr2026\ehr\Temp\d_rs_ddw_sysoptions_u16.txt")
# Clean null-ish
clean = []
for ch in text:
    o = ord(ch)
    if ch in "\r\n\t" or 32 <= o < 0xD800 or 0xE000 <= o <= 0xFFFD:
        clean.append(ch)
    else:
        clean.append(" ")
cleaned = "".join(clean)
out_path.write_text(cleaned[:150000], encoding="utf-8")
print("wrote", out_path, "len", len(cleaned))

# Extract values= lines
vals = re.findall(r'name="([^"]+)".{0,200}?values="([^"]*)"', cleaned, flags=re.S)
print("values pairs", len(vals))
for n, v in vals[:80]:
    print("FIELD", n, "=>", v[:200])

# Also look for dddw.name
dddw = re.findall(r'name="([^"]+)".{0,300}?dddw\.name=([^\s]+)', cleaned, flags=re.S)
print("dddw pairs", len(dddw))
for n, v in dddw[:40]:
    print("DDDW", n, "=>", v)

# table from SQL
sqls = re.findall(r'(?is)retrieve="([^"]{20,2000})"', cleaned)
print("retrieve count", len(sqls))
for s in sqls[:5]:
    print("SQL:", s[:500].replace("\r", " ").replace("\n", " "))
