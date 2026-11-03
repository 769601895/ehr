# -*- coding: utf-8 -*-
import re
from pathlib import Path

data = Path(r"E:\ehr2026\ehr\yuenan\hrs.pbl").read_bytes()

# Decode whole file as utf-16le (lossy) - PBL has mixed, but source chunks are u16
raw = data.decode("utf-16le", errors="ignore")

# Focus on window containing d_rs_ddw_sysoptions and 系统参数设置----人事
anchors = []
for kw in ["d_rs_ddw_sysoptions", "系统参数设置----人事", "人事参数设置"]:
    i = 0
    while True:
        j = raw.find(kw, i)
        if j < 0:
            break
        anchors.append((kw, j))
        i = j + len(kw)

print("anchors", anchors[:20])

# Take largest contiguous interesting region: from first 系统参数设置----人事
start = raw.find("系统参数设置----人事")
if start < 0:
    start = raw.find("d_rs_ddw_sysoptions")
end = start + 80000
region = raw[start:end]

# Normalize weird chars
region_n = "".join(ch if ch.isprintable() or ch in "\r\n\t" else " " for ch in region)
Path(r"E:\ehr2026\ehr\Temp\dw_region.txt").write_text(region_n, encoding="utf-8")

# PB column edit style: values="显示\t码/显示\t码/"
# Also: name=sys_xxx ... values=
patterns = [
    r'name=([a-zA-Z_][\w]*)\s[^}]*?values="([^"]*)"',
    r'column\([^)]*name=([a-zA-Z_][\w]*)[^)]*values="([^"]*)"',
    r'name="([a-zA-Z_][\w]*)"[^}]*?values="([^"]*)"',
]
found = {}
for pat in patterns:
    for m in re.finditer(pat, region_n, flags=re.S):
        found[m.group(1)] = m.group(2)

print("found values fields", len(found))
for k, v in sorted(found.items()):
    print(f"{k} = {v}")

# Also search values= occurrences and backtrack for name=
print("\n--- nearby scan ---")
for m in re.finditer(r'values="([^"]{1,500})"', region_n):
    left = region_n[max(0, m.start()-400):m.start()]
    names = re.findall(r'name=?["\']?([a-zA-Z_][\w]*)', left)
    name = names[-1] if names else "?"
    print(f"{name} => {m.group(1)[:180]}")

# text columns / ddlb
print("\n--- column names in region ---")
cols = sorted(set(re.findall(r'\b(sys_[a-z0-9_]+)\b', region_n)))
print("\n".join(cols))

# retrieve SQL
for m in re.finditer(r'retrieve="(.*?)"', region_n, flags=re.S):
    print("\nRETRIEVE:", m.group(1)[:800])
