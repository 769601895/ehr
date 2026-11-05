/*
 * 变更说明：初始化中国省→市主数据（地级；直辖市/港澳同名市节点）
 * 模块：hrProvince / hrCity
 * 日期：2026-11-05
 * 数据来源：项目内 mapGeojson（省/地级），直辖市与港澳台为同名市节点
 * 是否幂等：是（按 code 不存在则插入）
 * 前置：已执行 01-省市建表.sql
 */
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

IF OBJECT_ID(N'dbo.hr_province', N'U') IS NULL OR OBJECT_ID(N'dbo.hr_city', N'U') IS NULL
BEGIN
    RAISERROR(N'请先执行 01-省市建表.sql', 16, 1);
    RETURN;
END

-- ===== 省 =====
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province WHERE province_code = N'110000' AND IsDelete = 0)
INSERT INTO dbo.hr_province (Id, province_code, province_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000005001, N'110000', N'北京市', 110000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province WHERE province_code = N'120000' AND IsDelete = 0)
INSERT INTO dbo.hr_province (Id, province_code, province_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000005002, N'120000', N'天津市', 120000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province WHERE province_code = N'130000' AND IsDelete = 0)
INSERT INTO dbo.hr_province (Id, province_code, province_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000005003, N'130000', N'河北省', 130000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province WHERE province_code = N'140000' AND IsDelete = 0)
INSERT INTO dbo.hr_province (Id, province_code, province_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000005004, N'140000', N'山西省', 140000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province WHERE province_code = N'150000' AND IsDelete = 0)
INSERT INTO dbo.hr_province (Id, province_code, province_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000005005, N'150000', N'内蒙古自治区', 150000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province WHERE province_code = N'210000' AND IsDelete = 0)
INSERT INTO dbo.hr_province (Id, province_code, province_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000005006, N'210000', N'辽宁省', 210000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province WHERE province_code = N'220000' AND IsDelete = 0)
INSERT INTO dbo.hr_province (Id, province_code, province_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000005007, N'220000', N'吉林省', 220000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province WHERE province_code = N'230000' AND IsDelete = 0)
INSERT INTO dbo.hr_province (Id, province_code, province_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000005008, N'230000', N'黑龙江省', 230000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province WHERE province_code = N'310000' AND IsDelete = 0)
INSERT INTO dbo.hr_province (Id, province_code, province_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000005009, N'310000', N'上海市', 310000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province WHERE province_code = N'320000' AND IsDelete = 0)
INSERT INTO dbo.hr_province (Id, province_code, province_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000005010, N'320000', N'江苏省', 320000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province WHERE province_code = N'330000' AND IsDelete = 0)
INSERT INTO dbo.hr_province (Id, province_code, province_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000005011, N'330000', N'浙江省', 330000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province WHERE province_code = N'340000' AND IsDelete = 0)
INSERT INTO dbo.hr_province (Id, province_code, province_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000005012, N'340000', N'安徽省', 340000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province WHERE province_code = N'350000' AND IsDelete = 0)
INSERT INTO dbo.hr_province (Id, province_code, province_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000005013, N'350000', N'福建省', 350000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province WHERE province_code = N'360000' AND IsDelete = 0)
INSERT INTO dbo.hr_province (Id, province_code, province_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000005014, N'360000', N'江西省', 360000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province WHERE province_code = N'370000' AND IsDelete = 0)
INSERT INTO dbo.hr_province (Id, province_code, province_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000005015, N'370000', N'山东省', 370000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province WHERE province_code = N'410000' AND IsDelete = 0)
INSERT INTO dbo.hr_province (Id, province_code, province_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000005016, N'410000', N'河南省', 410000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province WHERE province_code = N'420000' AND IsDelete = 0)
INSERT INTO dbo.hr_province (Id, province_code, province_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000005017, N'420000', N'湖北省', 420000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province WHERE province_code = N'430000' AND IsDelete = 0)
INSERT INTO dbo.hr_province (Id, province_code, province_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000005018, N'430000', N'湖南省', 430000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province WHERE province_code = N'440000' AND IsDelete = 0)
INSERT INTO dbo.hr_province (Id, province_code, province_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000005019, N'440000', N'广东省', 440000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province WHERE province_code = N'450000' AND IsDelete = 0)
INSERT INTO dbo.hr_province (Id, province_code, province_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000005020, N'450000', N'广西壮族自治区', 450000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province WHERE province_code = N'460000' AND IsDelete = 0)
INSERT INTO dbo.hr_province (Id, province_code, province_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000005021, N'460000', N'海南省', 460000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province WHERE province_code = N'500000' AND IsDelete = 0)
INSERT INTO dbo.hr_province (Id, province_code, province_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000005022, N'500000', N'重庆市', 500000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province WHERE province_code = N'510000' AND IsDelete = 0)
INSERT INTO dbo.hr_province (Id, province_code, province_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000005023, N'510000', N'四川省', 510000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province WHERE province_code = N'520000' AND IsDelete = 0)
INSERT INTO dbo.hr_province (Id, province_code, province_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000005024, N'520000', N'贵州省', 520000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province WHERE province_code = N'530000' AND IsDelete = 0)
INSERT INTO dbo.hr_province (Id, province_code, province_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000005025, N'530000', N'云南省', 530000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province WHERE province_code = N'540000' AND IsDelete = 0)
INSERT INTO dbo.hr_province (Id, province_code, province_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000005026, N'540000', N'西藏自治区', 540000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province WHERE province_code = N'610000' AND IsDelete = 0)
INSERT INTO dbo.hr_province (Id, province_code, province_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000005027, N'610000', N'陕西省', 610000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province WHERE province_code = N'620000' AND IsDelete = 0)
INSERT INTO dbo.hr_province (Id, province_code, province_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000005028, N'620000', N'甘肃省', 620000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province WHERE province_code = N'630000' AND IsDelete = 0)
INSERT INTO dbo.hr_province (Id, province_code, province_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000005029, N'630000', N'青海省', 630000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province WHERE province_code = N'640000' AND IsDelete = 0)
INSERT INTO dbo.hr_province (Id, province_code, province_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000005030, N'640000', N'宁夏回族自治区', 640000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province WHERE province_code = N'650000' AND IsDelete = 0)
INSERT INTO dbo.hr_province (Id, province_code, province_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000005031, N'650000', N'新疆维吾尔自治区', 650000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province WHERE province_code = N'710000' AND IsDelete = 0)
INSERT INTO dbo.hr_province (Id, province_code, province_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000005032, N'710000', N'台湾省', 710000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province WHERE province_code = N'810000' AND IsDelete = 0)
INSERT INTO dbo.hr_province (Id, province_code, province_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000005033, N'810000', N'香港特别行政区', 810000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_province WHERE province_code = N'820000' AND IsDelete = 0)
INSERT INTO dbo.hr_province (Id, province_code, province_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000005034, N'820000', N'澳门特别行政区', 820000, N'Active', NULL, GETDATE(), 0);
-- ===== 市 =====
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'110100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006001, N'110000', N'110100', N'北京市', 110100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'120100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006002, N'120000', N'120100', N'天津市', 120100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'130100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006003, N'130000', N'130100', N'石家庄市', 130100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'130200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006004, N'130000', N'130200', N'唐山市', 130200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'130300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006005, N'130000', N'130300', N'秦皇岛市', 130300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'130400' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006006, N'130000', N'130400', N'邯郸市', 130400, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'130500' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006007, N'130000', N'130500', N'邢台市', 130500, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'130600' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006008, N'130000', N'130600', N'保定市', 130600, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'130700' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006009, N'130000', N'130700', N'张家口市', 130700, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'130800' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006010, N'130000', N'130800', N'承德市', 130800, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'130900' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006011, N'130000', N'130900', N'沧州市', 130900, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'131000' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006012, N'130000', N'131000', N'廊坊市', 131000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'131100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006013, N'130000', N'131100', N'衡水市', 131100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'140100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006014, N'140000', N'140100', N'太原市', 140100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'140200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006015, N'140000', N'140200', N'大同市', 140200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'140300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006016, N'140000', N'140300', N'阳泉市', 140300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'140400' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006017, N'140000', N'140400', N'长治市', 140400, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'140500' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006018, N'140000', N'140500', N'晋城市', 140500, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'140600' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006019, N'140000', N'140600', N'朔州市', 140600, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'140700' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006020, N'140000', N'140700', N'晋中市', 140700, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'140800' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006021, N'140000', N'140800', N'运城市', 140800, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'140900' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006022, N'140000', N'140900', N'忻州市', 140900, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'141000' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006023, N'140000', N'141000', N'临汾市', 141000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'141100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006024, N'140000', N'141100', N'吕梁市', 141100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'150100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006025, N'150000', N'150100', N'呼和浩特市', 150100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'150200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006026, N'150000', N'150200', N'包头市', 150200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'150300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006027, N'150000', N'150300', N'乌海市', 150300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'150400' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006028, N'150000', N'150400', N'赤峰市', 150400, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'150500' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006029, N'150000', N'150500', N'通辽市', 150500, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'150600' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006030, N'150000', N'150600', N'鄂尔多斯市', 150600, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'150700' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006031, N'150000', N'150700', N'呼伦贝尔市', 150700, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'150800' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006032, N'150000', N'150800', N'巴彦淖尔市', 150800, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'150900' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006033, N'150000', N'150900', N'乌兰察布市', 150900, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'152200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006034, N'150000', N'152200', N'兴安盟', 152200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'152500' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006035, N'150000', N'152500', N'锡林郭勒盟', 152500, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'152900' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006036, N'150000', N'152900', N'阿拉善盟', 152900, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'210100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006037, N'210000', N'210100', N'沈阳市', 210100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'210200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006038, N'210000', N'210200', N'大连市', 210200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'210300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006039, N'210000', N'210300', N'鞍山市', 210300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'210400' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006040, N'210000', N'210400', N'抚顺市', 210400, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'210500' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006041, N'210000', N'210500', N'本溪市', 210500, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'210600' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006042, N'210000', N'210600', N'丹东市', 210600, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'210700' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006043, N'210000', N'210700', N'锦州市', 210700, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'210800' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006044, N'210000', N'210800', N'营口市', 210800, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'210900' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006045, N'210000', N'210900', N'阜新市', 210900, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'211000' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006046, N'210000', N'211000', N'辽阳市', 211000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'211100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006047, N'210000', N'211100', N'盘锦市', 211100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'211200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006048, N'210000', N'211200', N'铁岭市', 211200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'211300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006049, N'210000', N'211300', N'朝阳市', 211300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'211400' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006050, N'210000', N'211400', N'葫芦岛市', 211400, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'220100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006051, N'220000', N'220100', N'长春市', 220100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'220200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006052, N'220000', N'220200', N'吉林市', 220200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'220300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006053, N'220000', N'220300', N'四平市', 220300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'220400' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006054, N'220000', N'220400', N'辽源市', 220400, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'220500' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006055, N'220000', N'220500', N'通化市', 220500, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'220600' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006056, N'220000', N'220600', N'白山市', 220600, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'220700' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006057, N'220000', N'220700', N'松原市', 220700, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'220800' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006058, N'220000', N'220800', N'白城市', 220800, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'222400' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006059, N'220000', N'222400', N'延边朝鲜族自治州', 222400, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'230100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006060, N'230000', N'230100', N'哈尔滨市', 230100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'230200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006061, N'230000', N'230200', N'齐齐哈尔市', 230200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'230300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006062, N'230000', N'230300', N'鸡西市', 230300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'230400' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006063, N'230000', N'230400', N'鹤岗市', 230400, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'230500' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006064, N'230000', N'230500', N'双鸭山市', 230500, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'230600' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006065, N'230000', N'230600', N'大庆市', 230600, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'230700' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006066, N'230000', N'230700', N'伊春市', 230700, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'230800' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006067, N'230000', N'230800', N'佳木斯市', 230800, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'230900' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006068, N'230000', N'230900', N'七台河市', 230900, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'231000' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006069, N'230000', N'231000', N'牡丹江市', 231000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'231100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006070, N'230000', N'231100', N'黑河市', 231100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'231200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006071, N'230000', N'231200', N'绥化市', 231200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'232700' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006072, N'230000', N'232700', N'大兴安岭地区', 232700, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'310100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006073, N'310000', N'310100', N'上海市', 310100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'320100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006074, N'320000', N'320100', N'南京市', 320100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'320200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006075, N'320000', N'320200', N'无锡市', 320200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'320300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006076, N'320000', N'320300', N'徐州市', 320300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'320400' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006077, N'320000', N'320400', N'常州市', 320400, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'320500' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006078, N'320000', N'320500', N'苏州市', 320500, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'320600' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006079, N'320000', N'320600', N'南通市', 320600, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'320700' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006080, N'320000', N'320700', N'连云港市', 320700, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'320800' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006081, N'320000', N'320800', N'淮安市', 320800, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'320900' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006082, N'320000', N'320900', N'盐城市', 320900, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'321000' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006083, N'320000', N'321000', N'扬州市', 321000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'321100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006084, N'320000', N'321100', N'镇江市', 321100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'321200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006085, N'320000', N'321200', N'泰州市', 321200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'321300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006086, N'320000', N'321300', N'宿迁市', 321300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'330100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006087, N'330000', N'330100', N'杭州市', 330100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'330200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006088, N'330000', N'330200', N'宁波市', 330200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'330300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006089, N'330000', N'330300', N'温州市', 330300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'330400' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006090, N'330000', N'330400', N'嘉兴市', 330400, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'330500' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006091, N'330000', N'330500', N'湖州市', 330500, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'330600' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006092, N'330000', N'330600', N'绍兴市', 330600, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'330700' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006093, N'330000', N'330700', N'金华市', 330700, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'330800' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006094, N'330000', N'330800', N'衢州市', 330800, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'330900' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006095, N'330000', N'330900', N'舟山市', 330900, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'331000' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006096, N'330000', N'331000', N'台州市', 331000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'331100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006097, N'330000', N'331100', N'丽水市', 331100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'340100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006098, N'340000', N'340100', N'合肥市', 340100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'340200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006099, N'340000', N'340200', N'芜湖市', 340200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'340300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006100, N'340000', N'340300', N'蚌埠市', 340300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'340400' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006101, N'340000', N'340400', N'淮南市', 340400, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'340500' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006102, N'340000', N'340500', N'马鞍山市', 340500, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'340600' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006103, N'340000', N'340600', N'淮北市', 340600, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'340700' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006104, N'340000', N'340700', N'铜陵市', 340700, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'340800' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006105, N'340000', N'340800', N'安庆市', 340800, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'341000' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006106, N'340000', N'341000', N'黄山市', 341000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'341100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006107, N'340000', N'341100', N'滁州市', 341100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'341200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006108, N'340000', N'341200', N'阜阳市', 341200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'341300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006109, N'340000', N'341300', N'宿州市', 341300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'341500' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006110, N'340000', N'341500', N'六安市', 341500, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'341600' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006111, N'340000', N'341600', N'亳州市', 341600, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'341700' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006112, N'340000', N'341700', N'池州市', 341700, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'341800' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006113, N'340000', N'341800', N'宣城市', 341800, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'350100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006114, N'350000', N'350100', N'福州市', 350100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'350200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006115, N'350000', N'350200', N'厦门市', 350200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'350300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006116, N'350000', N'350300', N'莆田市', 350300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'350400' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006117, N'350000', N'350400', N'三明市', 350400, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'350500' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006118, N'350000', N'350500', N'泉州市', 350500, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'350600' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006119, N'350000', N'350600', N'漳州市', 350600, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'350700' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006120, N'350000', N'350700', N'南平市', 350700, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'350800' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006121, N'350000', N'350800', N'龙岩市', 350800, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'350900' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006122, N'350000', N'350900', N'宁德市', 350900, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'360100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006123, N'360000', N'360100', N'南昌市', 360100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'360200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006124, N'360000', N'360200', N'景德镇市', 360200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'360300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006125, N'360000', N'360300', N'萍乡市', 360300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'360400' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006126, N'360000', N'360400', N'九江市', 360400, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'360500' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006127, N'360000', N'360500', N'新余市', 360500, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'360600' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006128, N'360000', N'360600', N'鹰潭市', 360600, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'360700' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006129, N'360000', N'360700', N'赣州市', 360700, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'360800' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006130, N'360000', N'360800', N'吉安市', 360800, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'360900' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006131, N'360000', N'360900', N'宜春市', 360900, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'361000' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006132, N'360000', N'361000', N'抚州市', 361000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'361100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006133, N'360000', N'361100', N'上饶市', 361100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'370100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006134, N'370000', N'370100', N'济南市', 370100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'370200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006135, N'370000', N'370200', N'青岛市', 370200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'370300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006136, N'370000', N'370300', N'淄博市', 370300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'370400' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006137, N'370000', N'370400', N'枣庄市', 370400, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'370500' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006138, N'370000', N'370500', N'东营市', 370500, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'370600' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006139, N'370000', N'370600', N'烟台市', 370600, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'370700' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006140, N'370000', N'370700', N'潍坊市', 370700, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'370800' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006141, N'370000', N'370800', N'济宁市', 370800, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'370900' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006142, N'370000', N'370900', N'泰安市', 370900, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'371000' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006143, N'370000', N'371000', N'威海市', 371000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'371100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006144, N'370000', N'371100', N'日照市', 371100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'371300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006145, N'370000', N'371300', N'临沂市', 371300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'371400' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006146, N'370000', N'371400', N'德州市', 371400, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'371500' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006147, N'370000', N'371500', N'聊城市', 371500, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'371600' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006148, N'370000', N'371600', N'滨州市', 371600, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'371700' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006149, N'370000', N'371700', N'菏泽市', 371700, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'410100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006150, N'410000', N'410100', N'郑州市', 410100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'410200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006151, N'410000', N'410200', N'开封市', 410200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'410300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006152, N'410000', N'410300', N'洛阳市', 410300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'410400' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006153, N'410000', N'410400', N'平顶山市', 410400, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'410500' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006154, N'410000', N'410500', N'安阳市', 410500, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'410600' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006155, N'410000', N'410600', N'鹤壁市', 410600, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'410700' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006156, N'410000', N'410700', N'新乡市', 410700, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'410800' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006157, N'410000', N'410800', N'焦作市', 410800, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'410900' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006158, N'410000', N'410900', N'濮阳市', 410900, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'411000' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006159, N'410000', N'411000', N'许昌市', 411000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'411100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006160, N'410000', N'411100', N'漯河市', 411100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'411200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006161, N'410000', N'411200', N'三门峡市', 411200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'411300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006162, N'410000', N'411300', N'南阳市', 411300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'411400' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006163, N'410000', N'411400', N'商丘市', 411400, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'411500' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006164, N'410000', N'411500', N'信阳市', 411500, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'411600' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006165, N'410000', N'411600', N'周口市', 411600, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'411700' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006166, N'410000', N'411700', N'驻马店市', 411700, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'419001' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006167, N'410000', N'419001', N'济源市', 419001, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'420100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006168, N'420000', N'420100', N'武汉市', 420100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'420200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006169, N'420000', N'420200', N'黄石市', 420200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'420300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006170, N'420000', N'420300', N'十堰市', 420300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'420500' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006171, N'420000', N'420500', N'宜昌市', 420500, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'420600' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006172, N'420000', N'420600', N'襄阳市', 420600, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'420700' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006173, N'420000', N'420700', N'鄂州市', 420700, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'420800' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006174, N'420000', N'420800', N'荆门市', 420800, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'420900' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006175, N'420000', N'420900', N'孝感市', 420900, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'421000' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006176, N'420000', N'421000', N'荆州市', 421000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'421100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006177, N'420000', N'421100', N'黄冈市', 421100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'421200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006178, N'420000', N'421200', N'咸宁市', 421200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'421300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006179, N'420000', N'421300', N'随州市', 421300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'422800' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006180, N'420000', N'422800', N'恩施土家族苗族自治州', 422800, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'429004' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006181, N'420000', N'429004', N'仙桃市', 429004, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'429005' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006182, N'420000', N'429005', N'潜江市', 429005, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'429006' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006183, N'420000', N'429006', N'天门市', 429006, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'429021' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006184, N'420000', N'429021', N'神农架林区', 429021, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'430100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006185, N'430000', N'430100', N'长沙市', 430100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'430200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006186, N'430000', N'430200', N'株洲市', 430200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'430300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006187, N'430000', N'430300', N'湘潭市', 430300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'430400' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006188, N'430000', N'430400', N'衡阳市', 430400, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'430500' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006189, N'430000', N'430500', N'邵阳市', 430500, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'430600' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006190, N'430000', N'430600', N'岳阳市', 430600, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'430700' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006191, N'430000', N'430700', N'常德市', 430700, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'430800' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006192, N'430000', N'430800', N'张家界市', 430800, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'430900' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006193, N'430000', N'430900', N'益阳市', 430900, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'431000' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006194, N'430000', N'431000', N'郴州市', 431000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'431100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006195, N'430000', N'431100', N'永州市', 431100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'431200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006196, N'430000', N'431200', N'怀化市', 431200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'431300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006197, N'430000', N'431300', N'娄底市', 431300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'433100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006198, N'430000', N'433100', N'湘西土家族苗族自治州', 433100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'440100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006199, N'440000', N'440100', N'广州市', 440100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'440200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006200, N'440000', N'440200', N'韶关市', 440200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'440300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006201, N'440000', N'440300', N'深圳市', 440300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'440400' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006202, N'440000', N'440400', N'珠海市', 440400, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'440500' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006203, N'440000', N'440500', N'汕头市', 440500, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'440600' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006204, N'440000', N'440600', N'佛山市', 440600, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'440700' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006205, N'440000', N'440700', N'江门市', 440700, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'440800' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006206, N'440000', N'440800', N'湛江市', 440800, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'440900' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006207, N'440000', N'440900', N'茂名市', 440900, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'441200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006208, N'440000', N'441200', N'肇庆市', 441200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'441300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006209, N'440000', N'441300', N'惠州市', 441300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'441400' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006210, N'440000', N'441400', N'梅州市', 441400, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'441500' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006211, N'440000', N'441500', N'汕尾市', 441500, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'441600' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006212, N'440000', N'441600', N'河源市', 441600, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'441700' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006213, N'440000', N'441700', N'阳江市', 441700, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'441800' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006214, N'440000', N'441800', N'清远市', 441800, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'441900' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006215, N'440000', N'441900', N'东莞市', 441900, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'442000' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006216, N'440000', N'442000', N'中山市', 442000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'445100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006217, N'440000', N'445100', N'潮州市', 445100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'445200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006218, N'440000', N'445200', N'揭阳市', 445200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'445300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006219, N'440000', N'445300', N'云浮市', 445300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'450100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006220, N'450000', N'450100', N'南宁市', 450100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'450200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006221, N'450000', N'450200', N'柳州市', 450200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'450300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006222, N'450000', N'450300', N'桂林市', 450300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'450400' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006223, N'450000', N'450400', N'梧州市', 450400, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'450500' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006224, N'450000', N'450500', N'北海市', 450500, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'450600' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006225, N'450000', N'450600', N'防城港市', 450600, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'450700' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006226, N'450000', N'450700', N'钦州市', 450700, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'450800' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006227, N'450000', N'450800', N'贵港市', 450800, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'450900' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006228, N'450000', N'450900', N'玉林市', 450900, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'451000' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006229, N'450000', N'451000', N'百色市', 451000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'451100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006230, N'450000', N'451100', N'贺州市', 451100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'451200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006231, N'450000', N'451200', N'河池市', 451200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'451300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006232, N'450000', N'451300', N'来宾市', 451300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'451400' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006233, N'450000', N'451400', N'崇左市', 451400, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'460100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006234, N'460000', N'460100', N'海口市', 460100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'460200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006235, N'460000', N'460200', N'三亚市', 460200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'460300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006236, N'460000', N'460300', N'三沙市', 460300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'460400' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006237, N'460000', N'460400', N'儋州市', 460400, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'469001' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006238, N'460000', N'469001', N'五指山市', 469001, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'469002' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006239, N'460000', N'469002', N'琼海市', 469002, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'469005' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006240, N'460000', N'469005', N'文昌市', 469005, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'469006' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006241, N'460000', N'469006', N'万宁市', 469006, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'469007' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006242, N'460000', N'469007', N'东方市', 469007, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'469021' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006243, N'460000', N'469021', N'定安县', 469021, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'469022' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006244, N'460000', N'469022', N'屯昌县', 469022, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'469023' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006245, N'460000', N'469023', N'澄迈县', 469023, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'469024' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006246, N'460000', N'469024', N'临高县', 469024, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'469025' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006247, N'460000', N'469025', N'白沙黎族自治县', 469025, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'469026' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006248, N'460000', N'469026', N'昌江黎族自治县', 469026, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'469027' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006249, N'460000', N'469027', N'乐东黎族自治县', 469027, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'469028' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006250, N'460000', N'469028', N'陵水黎族自治县', 469028, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'469029' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006251, N'460000', N'469029', N'保亭黎族苗族自治县', 469029, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'469030' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006252, N'460000', N'469030', N'琼中黎族苗族自治县', 469030, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'500100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006253, N'500000', N'500100', N'重庆市', 500100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'510100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006254, N'510000', N'510100', N'成都市', 510100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'510300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006255, N'510000', N'510300', N'自贡市', 510300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'510400' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006256, N'510000', N'510400', N'攀枝花市', 510400, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'510500' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006257, N'510000', N'510500', N'泸州市', 510500, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'510600' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006258, N'510000', N'510600', N'德阳市', 510600, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'510700' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006259, N'510000', N'510700', N'绵阳市', 510700, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'510800' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006260, N'510000', N'510800', N'广元市', 510800, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'510900' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006261, N'510000', N'510900', N'遂宁市', 510900, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'511000' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006262, N'510000', N'511000', N'内江市', 511000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'511100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006263, N'510000', N'511100', N'乐山市', 511100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'511300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006264, N'510000', N'511300', N'南充市', 511300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'511400' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006265, N'510000', N'511400', N'眉山市', 511400, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'511500' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006266, N'510000', N'511500', N'宜宾市', 511500, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'511600' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006267, N'510000', N'511600', N'广安市', 511600, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'511700' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006268, N'510000', N'511700', N'达州市', 511700, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'511800' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006269, N'510000', N'511800', N'雅安市', 511800, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'511900' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006270, N'510000', N'511900', N'巴中市', 511900, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'512000' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006271, N'510000', N'512000', N'资阳市', 512000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'513200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006272, N'510000', N'513200', N'阿坝藏族羌族自治州', 513200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'513300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006273, N'510000', N'513300', N'甘孜藏族自治州', 513300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'513400' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006274, N'510000', N'513400', N'凉山彝族自治州', 513400, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'520100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006275, N'520000', N'520100', N'贵阳市', 520100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'520200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006276, N'520000', N'520200', N'六盘水市', 520200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'520300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006277, N'520000', N'520300', N'遵义市', 520300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'520400' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006278, N'520000', N'520400', N'安顺市', 520400, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'520500' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006279, N'520000', N'520500', N'毕节市', 520500, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'520600' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006280, N'520000', N'520600', N'铜仁市', 520600, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'522300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006281, N'520000', N'522300', N'黔西南布依族苗族自治州', 522300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'522600' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006282, N'520000', N'522600', N'黔东南苗族侗族自治州', 522600, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'522700' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006283, N'520000', N'522700', N'黔南布依族苗族自治州', 522700, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'530100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006284, N'530000', N'530100', N'昆明市', 530100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'530300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006285, N'530000', N'530300', N'曲靖市', 530300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'530400' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006286, N'530000', N'530400', N'玉溪市', 530400, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'530500' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006287, N'530000', N'530500', N'保山市', 530500, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'530600' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006288, N'530000', N'530600', N'昭通市', 530600, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'530700' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006289, N'530000', N'530700', N'丽江市', 530700, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'530800' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006290, N'530000', N'530800', N'普洱市', 530800, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'530900' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006291, N'530000', N'530900', N'临沧市', 530900, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'532300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006292, N'530000', N'532300', N'楚雄彝族自治州', 532300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'532500' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006293, N'530000', N'532500', N'红河哈尼族彝族自治州', 532500, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'532600' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006294, N'530000', N'532600', N'文山壮族苗族自治州', 532600, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'532800' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006295, N'530000', N'532800', N'西双版纳傣族自治州', 532800, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'532900' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006296, N'530000', N'532900', N'大理白族自治州', 532900, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'533100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006297, N'530000', N'533100', N'德宏傣族景颇族自治州', 533100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'533300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006298, N'530000', N'533300', N'怒江傈僳族自治州', 533300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'533400' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006299, N'530000', N'533400', N'迪庆藏族自治州', 533400, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'540100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006300, N'540000', N'540100', N'拉萨市', 540100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'540200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006301, N'540000', N'540200', N'日喀则市', 540200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'540300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006302, N'540000', N'540300', N'昌都市', 540300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'540400' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006303, N'540000', N'540400', N'林芝市', 540400, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'540500' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006304, N'540000', N'540500', N'山南市', 540500, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'540600' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006305, N'540000', N'540600', N'那曲市', 540600, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'542500' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006306, N'540000', N'542500', N'阿里地区', 542500, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'610100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006307, N'610000', N'610100', N'西安市', 610100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'610200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006308, N'610000', N'610200', N'铜川市', 610200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'610300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006309, N'610000', N'610300', N'宝鸡市', 610300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'610400' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006310, N'610000', N'610400', N'咸阳市', 610400, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'610500' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006311, N'610000', N'610500', N'渭南市', 610500, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'610600' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006312, N'610000', N'610600', N'延安市', 610600, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'610700' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006313, N'610000', N'610700', N'汉中市', 610700, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'610800' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006314, N'610000', N'610800', N'榆林市', 610800, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'610900' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006315, N'610000', N'610900', N'安康市', 610900, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'611000' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006316, N'610000', N'611000', N'商洛市', 611000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'620100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006317, N'620000', N'620100', N'兰州市', 620100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'620200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006318, N'620000', N'620200', N'嘉峪关市', 620200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'620300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006319, N'620000', N'620300', N'金昌市', 620300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'620400' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006320, N'620000', N'620400', N'白银市', 620400, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'620500' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006321, N'620000', N'620500', N'天水市', 620500, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'620600' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006322, N'620000', N'620600', N'武威市', 620600, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'620700' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006323, N'620000', N'620700', N'张掖市', 620700, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'620800' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006324, N'620000', N'620800', N'平凉市', 620800, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'620900' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006325, N'620000', N'620900', N'酒泉市', 620900, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'621000' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006326, N'620000', N'621000', N'庆阳市', 621000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'621100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006327, N'620000', N'621100', N'定西市', 621100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'621200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006328, N'620000', N'621200', N'陇南市', 621200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'622900' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006329, N'620000', N'622900', N'临夏回族自治州', 622900, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'623000' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006330, N'620000', N'623000', N'甘南藏族自治州', 623000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'630100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006331, N'630000', N'630100', N'西宁市', 630100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'630200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006332, N'630000', N'630200', N'海东市', 630200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'632200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006333, N'630000', N'632200', N'海北藏族自治州', 632200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'632300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006334, N'630000', N'632300', N'黄南藏族自治州', 632300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'632500' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006335, N'630000', N'632500', N'海南藏族自治州', 632500, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'632600' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006336, N'630000', N'632600', N'果洛藏族自治州', 632600, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'632700' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006337, N'630000', N'632700', N'玉树藏族自治州', 632700, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'632800' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006338, N'630000', N'632800', N'海西蒙古族藏族自治州', 632800, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'640100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006339, N'640000', N'640100', N'银川市', 640100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'640200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006340, N'640000', N'640200', N'石嘴山市', 640200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'640300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006341, N'640000', N'640300', N'吴忠市', 640300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'640400' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006342, N'640000', N'640400', N'固原市', 640400, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'640500' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006343, N'640000', N'640500', N'中卫市', 640500, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'650100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006344, N'650000', N'650100', N'乌鲁木齐市', 650100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'650200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006345, N'650000', N'650200', N'克拉玛依市', 650200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'650400' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006346, N'650000', N'650400', N'吐鲁番市', 650400, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'650500' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006347, N'650000', N'650500', N'哈密市', 650500, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'652300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006348, N'650000', N'652300', N'昌吉回族自治州', 652300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'652700' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006349, N'650000', N'652700', N'博尔塔拉蒙古自治州', 652700, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'652800' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006350, N'650000', N'652800', N'巴音郭楞蒙古自治州', 652800, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'652900' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006351, N'650000', N'652900', N'阿克苏地区', 652900, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'653000' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006352, N'650000', N'653000', N'克孜勒苏柯尔克孜自治州', 653000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'653100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006353, N'650000', N'653100', N'喀什地区', 653100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'653200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006354, N'650000', N'653200', N'和田地区', 653200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'654000' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006355, N'650000', N'654000', N'伊犁哈萨克自治州', 654000, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'654200' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006356, N'650000', N'654200', N'塔城地区', 654200, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'654300' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006357, N'650000', N'654300', N'阿勒泰地区', 654300, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'659001' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006358, N'650000', N'659001', N'石河子市', 659001, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'659002' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006359, N'650000', N'659002', N'阿拉尔市', 659002, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'659003' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006360, N'650000', N'659003', N'图木舒克市', 659003, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'659004' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006361, N'650000', N'659004', N'五家渠市', 659004, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'659005' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006362, N'650000', N'659005', N'北屯市', 659005, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'659006' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006363, N'650000', N'659006', N'铁门关市', 659006, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'659007' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006364, N'650000', N'659007', N'双河市', 659007, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'659008' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006365, N'650000', N'659008', N'可克达拉市', 659008, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'659009' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006366, N'650000', N'659009', N'昆玉市', 659009, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'659010' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006367, N'650000', N'659010', N'胡杨河市', 659010, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'710100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006368, N'710000', N'710100', N'台湾省', 710100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'810100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006369, N'810000', N'810100', N'香港特别行政区', 810100, N'Active', NULL, GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM dbo.hr_city WHERE city_code = N'820100' AND IsDelete = 0)
INSERT INTO dbo.hr_city (Id, province_code, city_code, city_name, order_no, status, remark, CreateTime, IsDelete)
VALUES (1400000006370, N'820000', N'820100', N'澳门特别行政区', 820100, N'Active', NULL, GETDATE(), 0);
GO
DECLARE @pc INT; DECLARE @cc INT;
SELECT @pc = COUNT(*) FROM dbo.hr_province WHERE IsDelete=0;
SELECT @cc = COUNT(*) FROM dbo.hr_city WHERE IsDelete=0;
PRINT N'省市初始化完成：省 ' + CAST(@pc AS NVARCHAR(20)) + N'，市 ' + CAST(@cc AS NVARCHAR(20));
GO
