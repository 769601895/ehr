$ErrorActionPreference = "Stop"
$cs = "Server=jack\sql2014;Database=ehrnew;User Id=sa;Password=qingfeng;Encrypt=False;TrustServerCertificate=True;"
$conn = New-Object System.Data.SqlClient.SqlConnection $cs
$conn.Open()

function Set-Title([long]$id, [string]$title) {
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = "UPDATE SysMenu SET Title=@t WHERE Id=@id"
    [void]$cmd.Parameters.AddWithValue("@t", $title)
    [void]$cmd.Parameters.AddWithValue("@id", $id)
    [void]$cmd.ExecuteNonQuery()
}

Set-Title 1310000000102 ([string]::Concat([char]0x516C,[char]0x53F8,[char]0x53C2,[char]0x6570,[char]0x8BBE,[char]0x7F6E))
Set-Title 1310000000103 ([string]::Concat([char]0x67E5,[char]0x8BE2))
Set-Title 1310000000104 ([string]::Concat([char]0x65B0,[char]0x589E))
Set-Title 1310000000105 ([string]::Concat([char]0x4FDD,[char]0x5B58))
Set-Title 1310000000106 ([string]::Concat([char]0x5220,[char]0x9664))
Set-Title 1310000000107 ([string]::Concat([char]0x8BD1,[char]0x6587,[char]0x67E5,[char]0x8BE2))
Set-Title 1310000000108 ([string]::Concat([char]0x8BD1,[char]0x6587,[char]0x4FDD,[char]0x5B58))

$cmd3 = $conn.CreateCommand()
$cmd3.CommandText = "SELECT Id, Title, OrderNo FROM SysMenu WHERE Id IN (1310000000102,1310000000111) ORDER BY OrderNo"
$r = $cmd3.ExecuteReader()
while ($r.Read()) { Write-Output ("{0}|{1}|{2}" -f $r.GetInt64(0), $r.GetString(1), $r.GetInt32(2)) }
$r.Close(); $conn.Close()
