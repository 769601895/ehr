# -*- coding: utf-8 -*-
import re
from pathlib import Path

raw = Path(r"E:\ehr2026\ehr\yuenan\hrs.pbl").read_bytes().decode("utf-16le", errors="ignore")
start = raw.find("系统参数设置----人事")
region = raw[start : start + 120000]
region = "".join(
    ch if ch in "\r\n\t" or (32 <= ord(ch) <= 126) or (0x4E00 <= ord(ch) <= 0x9FFF) or ch in "（）【】≥≤:"
    else " "
    for ch in region
)
region = re.sub(r"[ ]{2,}", " ", region)

# text=(... name=xxx_t ... text="标签" ...) or text="标签" name=
labels = {}
for m in re.finditer(r'text=\((.*?)\)\s*(?=text=\(|column=\(|compute=\(|groupbox=\(|line=\(|rectangle=\(|$)', region, flags=re.S):
    body = m.group(1)
    nm = re.search(r"\bname=([a-zA-Z_][\w]*)", body)
    tx = re.search(r'text="([^"]*)"', body)
    if nm and tx:
        labels[nm.group(1)] = tx.group(1).strip().rstrip(":")

print("=== LABELS ===")
for k, v in sorted(labels.items(), key=lambda x: x[0]):
    print(f"{k}\t{v}")

# groupboxes
print("\n=== GROUPBOX ===")
for m in re.finditer(r'groupbox\([^)]*text="([^"]+)"', region):
    print(m.group(1))

# try_type values confirm
print("\n=== try_type snippets ===")
for m in re.finditer(r"sys_try_type.{0,200}", region):
    print(repr(m.group(0)[:200]))
