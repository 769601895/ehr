# -*- coding: utf-8 -*-
import re
from pathlib import Path

raw = Path(r"E:\ehr2026\ehr\yuenan\hrs.pbl").read_bytes().decode("utf-16le", "ignore")
start = raw.find("系统参数设置----人事")
region = raw[start : start + 120000]
region = "".join(
    ch if ch in "\r\n\t" or 32 <= ord(ch) <= 126 or 0x4E00 <= ord(ch) <= 0x9FFF or ch in "（）【】≥≤:"
    else " "
    for ch in region
)
seen = []
for m in re.finditer(r'text="([^"]{1,60})"', region):
    t = m.group(1).strip()
    if any("\u4e00" <= c <= "\u9fff" for c in t) and t not in seen:
        seen.append(t)
        print(t)
