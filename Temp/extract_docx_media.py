import zipfile, os, re
from xml.etree import ElementTree as ET

path = r"c:\Users\jack\Desktop\公司参数设置模块.docx"
out = r"e:\ehr2026\ehr\Temp\company_param_docx"
os.makedirs(out, exist_ok=True)

with zipfile.ZipFile(path) as z:
    for n in z.namelist():
        if n.startswith("word/media/"):
            target = os.path.join(out, os.path.basename(n))
            with open(target, "wb") as f:
                f.write(z.read(n))
            print("media:", target, os.path.getsize(target))

    doc = z.read("word/document.xml").decode("utf-8")
    rels_xml = z.read("word/_rels/document.xml.rels")
    root = ET.fromstring(rels_xml)
    ns = {"r": "http://schemas.openxmlformats.org/package/2006/relationships"}
    rid_map = {}
    for rel in root:
        rid = rel.attrib.get("Id")
        target = rel.attrib.get("Target")
        rid_map[rid] = target
        if target and "media" in target:
            print("rel", rid, "->", target)

    embeds = re.findall(r'r:embed="(rId\d+)"', doc)
    print("embed order:", embeds)
    for i, rid in enumerate(embeds, 1):
        print(f"  image#{i}: {rid} -> {rid_map.get(rid)}")
