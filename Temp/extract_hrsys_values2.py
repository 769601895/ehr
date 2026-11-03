# -*- coding: utf-8 -*-
"""Pair DW column name with values= using narrow windows; strip PBL binary junk."""
import re
from pathlib import Path

data = Path(r"E:\ehr2026\ehr\yuenan\hrs.pbl").read_bytes()
raw = data.decode("utf-16le", errors="ignore")

start = raw.find("系统参数设置----人事")
region = raw[start : start + 100000]
# Drop private-use / control except whitespace; also drop common PBL junk markers
region = "".join(
    ch if (ch in "\r\n\t" or (32 <= ord(ch) <= 126) or (0x4E00 <= ord(ch) <= 0x9FFF) or ch in "（）【】≥>≤±") else " "
    for ch in region
)
# collapse spaces but keep newlines
region = re.sub(r"[^\S\r\n]+", " ", region)
Path(r"E:\ehr2026\ehr\Temp\dw_region_clean.txt").write_text(region, encoding="utf-8")

# Strategy: find all values="..." then look back for last name=sys_ or name= within 300 chars
# Prefer name=sys_*
pairs = []
for m in re.finditer(r'values="([^"]*)"', region):
    left = region[max(0, m.start() - 500) : m.start()]
    # strip table qualifier noise
    names = re.findall(r"(?:^|[\s(])name=([a-zA-Z_][\w]*)", left)
    # filter out table names
    names = [n for n in names if n not in ("hr_sys_mstr", "datawindow", "column", "text", "compute")]
    name = names[-1] if names else None
    val = m.group(1).strip()
    # clean residual spaces in value from junk removal
    val = re.sub(r"\s+", "", val) if "\t" not in m.group(1) else m.group(1)
    # Prefer original with tabs
    val = m.group(1)
    val = re.sub(r" +", "", val)  # remove spaces introduced by junk
    pairs.append((name, val, m.start()))

# Deduplicate by name keeping last (edit style often after column)
by_name = {}
for name, val, pos in pairs:
    if not name:
        continue
    by_name[name] = val

print("=== PAIRED VALUES (%d) ===" % len(by_name))
for k in sorted(by_name):
    v = by_name[k]
    # pretty print code/label
    items = []
    for part in v.strip("/").split("/"):
        if not part.strip():
            continue
        if "\t" in part:
            lab, code = part.split("\t", 1)
            items.append(f"{code}={lab}")
        else:
            items.append(part)
    print(f"{k}: {items}")

# Also extract text= labels near column names for UI captions
print("\n=== TEXT LABELS (sample) ===")
# PB: text(name=sys_emp_id_t text="工号编码方式" ...)
for m in re.finditer(r'name=(sys_[a-z0-9_]+_t)\s[^}]*?text="([^"]+)"', region):
    print(m.group(1), "=>", m.group(2))
for m in re.finditer(r'text="([^"]+)"\s[^}]*?name=(sys_[a-z0-9_]+_t)', region):
    print(m.group(2), "=>", m.group(1))

# retrieve full
m = re.search(r'retrieve="(.*?)"\s*(?:arguments|update|table|)', region, flags=re.S)
if m:
    sql = re.sub(r"\s+", " ", m.group(1))
    Path(r"E:\ehr2026\ehr\Temp\dw_retrieve.sql").write_text(sql, encoding="utf-8")
    print("\nSQL cols:", len(re.findall(r"hr_sys_mstr\.([a-z0-9_]+)", sql, flags=re.I)))
    print(re.findall(r"hr_sys_mstr\.([a-z0-9_]+)", sql, flags=re.I))
