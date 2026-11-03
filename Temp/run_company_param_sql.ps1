$ErrorActionPreference = "Stop"
$cs = "Server=jack\sql2014;Database=ehrnew;User Id=sa;Password=qingfeng;Encrypt=False;TrustServerCertificate=True;"
$conn = New-Object System.Data.SqlClient.SqlConnection $cs
$conn.Open()

function Exec-Sql([string]$sql) {
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = $sql
    $cmd.CommandTimeout = 120
    [void]$cmd.ExecuteNonQuery()
}

# Fix index + lang table
Exec-Sql @"
SET QUOTED_IDENTIFIER ON;
IF OBJECT_ID(N'dbo.HrCompanyParamLang', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.HrCompanyParamLang
    (
        ParamId   BIGINT        NOT NULL,
        Culture   NVARCHAR(16)  NOT NULL,
        SiteName  NVARCHAR(100) NULL,
        CONSTRAINT PK_HrCompanyParamLang PRIMARY KEY CLUSTERED (ParamId, Culture)
    );
END
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_HrCompanyParam_SiteCode' AND object_id = OBJECT_ID(N'dbo.HrCompanyParam'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_HrCompanyParam_SiteCode
        ON dbo.HrCompanyParam (SiteCode)
        WHERE IsDelete = 0;
END
"@
Write-Host "DDL ok"

# Seed from generator DATA inline
$seedPy = Join-Path $PSScriptRoot "gen_company_param_codes.py"
# Re-parse DATA by executing python to emit TSV
$tsv = python -c @"
import importlib.util, sys
spec = importlib.util.spec_from_file_location('g', r'$($seedPy.Replace('\','\\'))')
m = importlib.util.module_from_spec(spec); spec.loader.exec_module(m)
for field, rows in m.DATA.items():
    for val, zh, en, sort in rows:
        print(f'{field}\t{val}\t{zh}\t{en}\t{sort}')
"@

$n = 0
foreach ($line in ($tsv -split "`n")) {
    $line = $line.Trim()
    if (-not $line) { continue }
    $p = $line -split "`t"
    if ($p.Length -lt 5) { continue }
    $field,$val,$zh,$en,$sort = $p[0],$p[1],$p[2],$p[3],$p[4]
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = @"
IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr WHERE code_field=@f AND code_value=@v)
  INSERT INTO dbo.code_mstr(code_field,code_value,code_desc,code_sort) VALUES(@f,@v,@zh,@s);
ELSE
  UPDATE dbo.code_mstr SET code_desc=@zh, code_sort=@s WHERE code_field=@f AND code_value=@v;

IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=@f AND code_value=@v AND culture=N'zh-CN')
  INSERT INTO dbo.code_mstr_lang(code_field,code_value,culture,code_desc) VALUES(@f,@v,N'zh-CN',@zh);
ELSE
  UPDATE dbo.code_mstr_lang SET code_desc=@zh WHERE code_field=@f AND code_value=@v AND culture=N'zh-CN';

IF NOT EXISTS (SELECT 1 FROM dbo.code_mstr_lang WHERE code_field=@f AND code_value=@v AND culture=N'en')
  INSERT INTO dbo.code_mstr_lang(code_field,code_value,culture,code_desc) VALUES(@f,@v,N'en',@en);
ELSE
  UPDATE dbo.code_mstr_lang SET code_desc=@en WHERE code_field=@f AND code_value=@v AND culture=N'en';
"@
    [void]$cmd.Parameters.AddWithValue("@f", $field)
    [void]$cmd.Parameters.AddWithValue("@v", $val)
    [void]$cmd.Parameters.AddWithValue("@zh", $zh)
    [void]$cmd.Parameters.AddWithValue("@en", $en)
    [void]$cmd.Parameters.AddWithValue("@s", $sort)
    [void]$cmd.ExecuteNonQuery()
    $n++
}
Write-Host "Seed rows upserted: $n"

$cmd = $conn.CreateCommand()
$cmd.CommandText = "SELECT COUNT(*) FROM code_mstr WHERE code_field='EmpIdMode'; SELECT OBJECT_ID('HrCompanyParamLang'); SELECT name FROM sys.indexes WHERE object_id=OBJECT_ID('HrCompanyParam') AND name='UX_HrCompanyParam_SiteCode';"
$r = $cmd.ExecuteReader()
$r.Read() | Out-Null; Write-Host "EmpIdMode count:" $r.GetValue(0)
$r.NextResult() | Out-Null; $r.Read() | Out-Null; Write-Host "Lang table object_id:" $r.GetValue(0)
$r.NextResult() | Out-Null
if ($r.Read()) { Write-Host "Index:" $r.GetValue(0) } else { Write-Host "Index missing" }
$r.Close()
$conn.Close()
