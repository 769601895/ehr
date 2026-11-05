/*
 * 变更说明：省市泰语/越南语译文种子；市名暂用英文国际名（与 en 一致），省名用泰/越惯用译名
 * 模块：hrProvince / hrCity
 * 日期：2026-11-05
 * 是否幂等：是
 */
SET NOCOUNT ON;
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'110000' AND culture=N'th') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'110000',N'th',N'ปักกิ่ง');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'110000' AND culture=N'vi') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'110000',N'vi',N'Bắc Kinh');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'120000' AND culture=N'th') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'120000',N'th',N'เทียนจิน');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'120000' AND culture=N'vi') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'120000',N'vi',N'Thiên Tân');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'130000' AND culture=N'th') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'130000',N'th',N'เหอเป่ย์');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'130000' AND culture=N'vi') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'130000',N'vi',N'Hà Bắc');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'140000' AND culture=N'th') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'140000',N'th',N'ซานซี');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'140000' AND culture=N'vi') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'140000',N'vi',N'Sơn Tây');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'150000' AND culture=N'th') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'150000',N'th',N'มองโกเลียใน');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'150000' AND culture=N'vi') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'150000',N'vi',N'Nội Mông');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'210000' AND culture=N'th') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'210000',N'th',N'เหลียวหนิง');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'210000' AND culture=N'vi') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'210000',N'vi',N'Liêu Ninh');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'220000' AND culture=N'th') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'220000',N'th',N'จี๋หลิน');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'220000' AND culture=N'vi') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'220000',N'vi',N'Cát Lâm');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'230000' AND culture=N'th') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'230000',N'th',N'เฮย์หลงเจียง');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'230000' AND culture=N'vi') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'230000',N'vi',N'Hắc Long Giang');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'310000' AND culture=N'th') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'310000',N'th',N'เซี่ยงไฮ้');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'310000' AND culture=N'vi') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'310000',N'vi',N'Thượng Hải');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'320000' AND culture=N'th') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'320000',N'th',N'เจียงซู');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'320000' AND culture=N'vi') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'320000',N'vi',N'Giang Tô');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'330000' AND culture=N'th') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'330000',N'th',N'เจ้อเจียง');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'330000' AND culture=N'vi') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'330000',N'vi',N'Chiết Giang');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'340000' AND culture=N'th') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'340000',N'th',N'อานฮุย');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'340000' AND culture=N'vi') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'340000',N'vi',N'An Huy');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'350000' AND culture=N'th') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'350000',N'th',N'ฝูเจี้ยน');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'350000' AND culture=N'vi') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'350000',N'vi',N'Phúc Kiến');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'360000' AND culture=N'th') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'360000',N'th',N'เจียงซี');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'360000' AND culture=N'vi') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'360000',N'vi',N'Giang Tây');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'370000' AND culture=N'th') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'370000',N'th',N'ซานตง');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'370000' AND culture=N'vi') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'370000',N'vi',N'Sơn Đông');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'410000' AND culture=N'th') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'410000',N'th',N'เหอหนาน');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'410000' AND culture=N'vi') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'410000',N'vi',N'Hà Nam');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'420000' AND culture=N'th') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'420000',N'th',N'หูเป่ย์');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'420000' AND culture=N'vi') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'420000',N'vi',N'Hồ Bắc');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'430000' AND culture=N'th') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'430000',N'th',N'หูหนาน');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'430000' AND culture=N'vi') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'430000',N'vi',N'Hồ Nam');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'440000' AND culture=N'th') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'440000',N'th',N'กวางตุ้ง');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'440000' AND culture=N'vi') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'440000',N'vi',N'Quảng Đông');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'450000' AND culture=N'th') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'450000',N'th',N'กวางซี');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'450000' AND culture=N'vi') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'450000',N'vi',N'Quảng Tây');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'460000' AND culture=N'th') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'460000',N'th',N'ไหหลำ');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'460000' AND culture=N'vi') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'460000',N'vi',N'Hải Nam');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'500000' AND culture=N'th') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'500000',N'th',N'ฉงชิ่ง');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'500000' AND culture=N'vi') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'500000',N'vi',N'Trùng Khánh');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'510000' AND culture=N'th') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'510000',N'th',N'เสฉวน');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'510000' AND culture=N'vi') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'510000',N'vi',N'Tứ Xuyên');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'520000' AND culture=N'th') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'520000',N'th',N'กุ้ยโจว');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'520000' AND culture=N'vi') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'520000',N'vi',N'Quý Châu');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'530000' AND culture=N'th') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'530000',N'th',N'ยูนนาน');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'530000' AND culture=N'vi') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'530000',N'vi',N'Vân Nam');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'540000' AND culture=N'th') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'540000',N'th',N'ทิเบต');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'540000' AND culture=N'vi') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'540000',N'vi',N'Tây Tạng');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'610000' AND culture=N'th') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'610000',N'th',N'ส่านซี');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'610000' AND culture=N'vi') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'610000',N'vi',N'Thiểm Tây');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'620000' AND culture=N'th') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'620000',N'th',N'กานซู่');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'620000' AND culture=N'vi') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'620000',N'vi',N'Cam Túc');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'630000' AND culture=N'th') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'630000',N'th',N'ชิงไห่');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'630000' AND culture=N'vi') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'630000',N'vi',N'Thanh Hải');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'640000' AND culture=N'th') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'640000',N'th',N'หนิงเซี่ย');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'640000' AND culture=N'vi') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'640000',N'vi',N'Ninh Hạ');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'650000' AND culture=N'th') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'650000',N'th',N'ซินเจียง');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'650000' AND culture=N'vi') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'650000',N'vi',N'Tân Cương');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'710000' AND culture=N'th') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'710000',N'th',N'ไต้หวัน');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'710000' AND culture=N'vi') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'710000',N'vi',N'Đài Loan');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'810000' AND culture=N'th') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'810000',N'th',N'ฮ่องกง');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'810000' AND culture=N'vi') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'810000',N'vi',N'Hồng Kông');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'820000' AND culture=N'th') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'820000',N'th',N'มาเก๊า');
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province_lang WHERE province_code=N'820000' AND culture=N'vi') INSERT INTO dbo.hr_province_lang(province_code,culture,province_name) VALUES (N'820000',N'vi',N'Ma Cao');
-- 市：先从 en 复制到 th/vi（国际通用城市英文名，避免泰语界面仍显示中文）
INSERT INTO dbo.hr_city_lang (city_code, culture, city_name)
SELECT e.city_code, N'th', e.city_name
FROM dbo.hr_city_lang e
WHERE e.culture = N'en'
  AND NOT EXISTS (SELECT 1 FROM dbo.hr_city_lang t WHERE t.city_code = e.city_code AND t.culture = N'th');

INSERT INTO dbo.hr_city_lang (city_code, culture, city_name)
SELECT e.city_code, N'vi', e.city_name
FROM dbo.hr_city_lang e
WHERE e.culture = N'en'
  AND NOT EXISTS (SELECT 1 FROM dbo.hr_city_lang t WHERE t.city_code = e.city_code AND t.culture = N'vi');
UPDATE dbo.hr_city_lang SET city_name=N'ปักกิ่ง' WHERE city_code=N'110100' AND culture=N'th';
UPDATE dbo.hr_city_lang SET city_name=N'Bắc Kinh' WHERE city_code=N'110100' AND culture=N'vi';
UPDATE dbo.hr_city_lang SET city_name=N'เทียนจิน' WHERE city_code=N'120100' AND culture=N'th';
UPDATE dbo.hr_city_lang SET city_name=N'Thiên Tân' WHERE city_code=N'120100' AND culture=N'vi';
UPDATE dbo.hr_city_lang SET city_name=N'เซี่ยงไฮ้' WHERE city_code=N'310100' AND culture=N'th';
UPDATE dbo.hr_city_lang SET city_name=N'Thượng Hải' WHERE city_code=N'310100' AND culture=N'vi';
UPDATE dbo.hr_city_lang SET city_name=N'ฉงชิ่ง' WHERE city_code=N'500100' AND culture=N'th';
UPDATE dbo.hr_city_lang SET city_name=N'Trùng Khánh' WHERE city_code=N'500100' AND culture=N'vi';
UPDATE dbo.hr_city_lang SET city_name=N'ไต้หวัน' WHERE city_code=N'710100' AND culture=N'th';
UPDATE dbo.hr_city_lang SET city_name=N'Đài Loan' WHERE city_code=N'710100' AND culture=N'vi';
UPDATE dbo.hr_city_lang SET city_name=N'ฮ่องกง' WHERE city_code=N'810100' AND culture=N'th';
UPDATE dbo.hr_city_lang SET city_name=N'Hồng Kông' WHERE city_code=N'810100' AND culture=N'vi';
UPDATE dbo.hr_city_lang SET city_name=N'มาเก๊า' WHERE city_code=N'820100' AND culture=N'th';
UPDATE dbo.hr_city_lang SET city_name=N'Ma Cao' WHERE city_code=N'820100' AND culture=N'vi';
PRINT N'th/vi 省市译文已就绪';
GO
