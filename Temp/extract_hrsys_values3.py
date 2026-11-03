# -*- coding: utf-8 -*-
import re
from pathlib import Path

raw = Path(r"E:\ehr2026\ehr\yuenan\hrs.pbl").read_bytes().decode("utf-16le", errors="ignore")
start = raw.find("系统参数设置----人事")
region = raw[start : start + 120000]
# Keep tabs, ASCII, CJK; replace else with space
region = "".join(
    ch if ch in "\r\n\t" or (32 <= ord(ch) <= 126) or (0x4E00 <= ord(ch) <= 0x9FFF) or ch in "（）【】≥≤"
    else " "
    for ch in region
)
region = re.sub(r"[ ]{2,}", " ", region)
Path(r"E:\ehr2026\ehr\Temp\dw_region_clean.txt").write_text(region, encoding="utf-8")

# Extract column=( ... ) blocks - non-greedy until next column=( or text(
# PB format often: column=(type=... name=xxx ... values="..." ...)
blocks = list(re.finditer(r"column=\((.*?)\)\s*(?=column=\(|text=\(|compute=\(|html|groupbox|line\(|rectangle\(|datawindow\(|$)", region, flags=re.S))
print("column blocks", len(blocks))

results = []
for b in blocks:
    body = b.group(1)
    nm = re.search(r"\bname=([a-zA-Z_][\w]*)", body)
    if not nm:
        continue
    name = nm.group(1)
    vals = re.search(r'values="([^"]*)"', body)
    dddw = re.search(r"dddw\.name=([^\s)]+)", body)
    edit = re.search(r"\bedit\.style=([^\s)]+)|editstyle=([^\s)]+)|style\.type=([^\s)]+)", body)
    results.append({
        "name": name,
        "values": vals.group(1) if vals else None,
        "dddw": dddw.group(1) if dddw else None,
        "edit": (edit.group(1) or edit.group(2) or edit.group(3)) if edit else None,
        "body_snip": body[:120].replace("\n", " "),
    })

# print dropdowns
print("\n=== DROPDOWNS ===")
for r in results:
    if r["values"] or r["dddw"]:
        if r["values"]:
            items = []
            for part in r["values"].strip("/").split("/"):
                if not part:
                    continue
                part = part.replace(" ", "")
                if "\t" in part:
                    lab, code = part.split("\t", 1)
                    items.append(f"{code}:{lab}")
                else:
                    items.append(part)
            print(f"{r['name']}\tVALUES\t{items}")
        if r["dddw"]:
            print(f"{r['name']}\tDDDW\t{r['dddw']}")

print("\n=== ALL COLUMNS ===")
for r in results:
    kind = "ddlb" if r["values"] else ("dddw" if r["dddw"] else "edit")
    print(f"{r['name']}\t{kind}")

# Also dump values= with 80 chars before for manual audit
print("\n=== RAW VALUES CONTEXT ===")
for m in re.finditer(r'values="([^"]*)"', region):
    ctx = region[max(0, m.start()-80):m.start()]
    ctx = re.sub(r"\s+", " ", ctx)
    print(f"...{ctx[-60:]} => {m.group(1)[:100]}")
