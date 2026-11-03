$ErrorActionPreference = "Stop"
$cs = "Server=jack\sql2014;Database=ehrnew;User Id=sa;Password=qingfeng;Encrypt=False;TrustServerCertificate=True;"
$conn = New-Object System.Data.SqlClient.SqlConnection $cs
$conn.Open()
$title = [string]::Concat(
    [char]0x516C, [char]0x53F8, [char]0x53C2, [char]0x6570, [char]0x8BBE, [char]0x7F6E
)
$cmd = $conn.CreateCommand()
$cmd.CommandText = "UPDATE SysMenu SET Title=@t WHERE Id=1310000000102"
[void]$cmd.Parameters.AddWithValue("@t", $title)
[void]$cmd.ExecuteNonQuery()

# also fix button titles that may be garbled
$map = @{
    1310000000103 = ([string]::Concat([char]0x67E5,[char]0x8BE2))
    1310000000104 = ([string]::Concat([char]0x65B0,[char]0x589E))
    1310000000105 = ([string]::Concat([char]0x4FDD,[char]0x5B58))
    1310000000106 = ([string]::Concat([char]0x5220,[char]0x9664))
    1310000000107 = ([string]::Concat([char]0x8BD1,[char]0x6587,[char]0x67E5,[char]0x8BE2))
    1310000000108 = ([string]::Concat([char]0x8BD1,[char]0x6587,[char]0x4FDD,[char]0x5B58))
}
foreach ($kv in $map.GetEnumerator()) {
    $c2 = $conn.CreateCommand()
    $c2.CommandText = "UPDATE SysMenu SET Title=@t WHERE Id=@id"
    [void]$c2.Parameters.AddWithValue("@t", $kv.Value)
    [void]$c2.Parameters.AddWithValue("@id", [int64]$kv.Key)
    [void]$c2.ExecuteNonQuery()
}

$cmd3 = $conn.CreateCommand()
$cmd3.CommandText = "SELECT Id, Title FROM SysMenu WHERE Id BETWEEN 1310000000102 AND 1310000000108 ORDER BY Id"
$r = $cmd3.ExecuteReader()
while ($r.Read()) { Write-Output ("{0}={1}" -f $r.GetInt64(0), $r.GetString(1)) }
$r.Close(); $conn.Close()
