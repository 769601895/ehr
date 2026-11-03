/*
 * 变更说明：工号编码方式 EmpIdMode 删除界面红框码值
 * 删除：2=2位年+流水号、3=4位年+流水号、6=1开始流水号、7=2位年+月+流水号
 * 保留：0自动流水号、1手工维护、4固定号+流水号、5=4位年+月+流水号
 * 日期：2026-10-31
 * 影响库：ehrnew（HR）
 * 是否幂等：是
 */
SET NOCOUNT ON;

DELETE FROM dbo.code_mstr_lang
WHERE code_field = N'EmpIdMode' AND code_value IN (N'2', N'3', N'6', N'7');

DELETE FROM dbo.code_mstr
WHERE code_field = N'EmpIdMode' AND code_value IN (N'2', N'3', N'6', N'7');
GO
