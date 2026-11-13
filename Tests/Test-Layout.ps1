<#
.SYNOPSIS
  Offline test of the ship layout data (Source/ShipLayout.cs) and of the room-name keys. No game started.
.DESCRIPTION
  Reads the rooms and doors from ShipLayout.cs and checks what would otherwise break the map in play:
    1 rooms     17 rooms, unique names, all inside the 95 x 47 layout
    2 apart     no two rooms overlap or touch: at least one wall cell between them
    3 doors     each door cell is outside every room and has a room on both sides (west and east, or south and north), two different rooms
    4 connected every room is reachable from the Cafeteria through doors
    5 keys      every room has a BS_Room_<Name> key in English and in French Keyed
  Exit code = number of failed checks.
.EXAMPLE
  powershell -NoProfile -ExecutionPolicy Bypass -File Tests/Test-Layout.ps1
#>
$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$src = Get-Content (Join-Path $root 'Source/ShipLayout.cs') -Raw

$rooms = @([regex]::Matches($src, 'new Room\("(\w+)",\s*(\d+),\s*(\d+),\s*(\d+),\s*(\d+)\)') | ForEach-Object {
  [pscustomobject]@{ Name = $_.Groups[1].Value; X0 = [int]$_.Groups[2].Value; Z0 = [int]$_.Groups[3].Value; X1 = [int]$_.Groups[4].Value; Z1 = [int]$_.Groups[5].Value }
})
$doors = @([regex]::Matches($src, 'new IntVec2\((\d+),\s*(\d+)\)') | ForEach-Object {
  [pscustomobject]@{ X = [int]$_.Groups[1].Value; Z = [int]$_.Groups[2].Value }
})
$width = [int]([regex]::Match($src, 'Width = (\d+)').Groups[1].Value)
$height = [int]([regex]::Match($src, 'Height = (\d+)').Groups[1].Value)

function RoomAt($x, $z) { $rooms | Where-Object { $x -ge $_.X0 -and $x -le $_.X1 -and $z -ge $_.Z0 -and $z -le $_.Z1 } | Select-Object -First 1 }

$failed = 0
function Check($n, $label, $problems) {
  if ($problems.Count -eq 0) { Write-Host "  $n. ok    $label" }
  else { $script:failed++; Write-Host "  $n. FAIL  $label"; $problems | ForEach-Object { Write-Host "        $_" } }
}

Write-Host "Test-Layout: $($rooms.Count) rooms, $($doors.Count) doors, layout ${width} x ${height}"

$p = @()
if ($rooms.Count -ne 17) { $p += "expected 17 rooms, found $($rooms.Count)" }
$dup = $rooms | Group-Object Name | Where-Object Count -gt 1
foreach ($d in $dup) { $p += "duplicate room name $($d.Name)" }
foreach ($r in $rooms) {
  if ($r.X0 -gt $r.X1 -or $r.Z0 -gt $r.Z1) { $p += "$($r.Name): empty rectangle" }
  if ($r.X0 -lt 0 -or $r.Z0 -lt 0 -or $r.X1 -ge $width -or $r.Z1 -ge $height) { $p += "$($r.Name): outside the layout" }
}
Check 1 'rooms: count, unique names, inside the layout' $p

$p = @()
for ($i = 0; $i -lt $rooms.Count; $i++) {
  for ($j = $i + 1; $j -lt $rooms.Count; $j++) {
    $a = $rooms[$i]; $b = $rooms[$j]
    $gapX = [Math]::Max($b.X0 - $a.X1, $a.X0 - $b.X1) - 1
    $gapZ = [Math]::Max($b.Z0 - $a.Z1, $a.Z0 - $b.Z1) - 1
    if ([Math]::Max($gapX, $gapZ) -lt 1) { $p += "$($a.Name) and $($b.Name) overlap or touch (no wall cell between)" }
  }
}
Check 2 'apart: one wall cell at least between two rooms' $p

$p = @()
$edges = @()
foreach ($d in $doors) {
  $label = "door ($($d.X), $($d.Z))"
  if (RoomAt $d.X $d.Z) { $p += "$label is inside a room"; continue }
  $w = RoomAt ($d.X - 1) $d.Z; $e = RoomAt ($d.X + 1) $d.Z
  $s = RoomAt $d.X ($d.Z - 1); $n = RoomAt $d.X ($d.Z + 1)
  if ($w -and $e -and $w.Name -ne $e.Name) { $edges += , @($w.Name, $e.Name) }
  elseif ($s -and $n -and $s.Name -ne $n.Name) { $edges += , @($s.Name, $n.Name) }
  else { $p += "$label has no two different rooms on opposite sides" }
}
$dupDoor = $doors | Group-Object { "$($_.X),$($_.Z)" } | Where-Object Count -gt 1
foreach ($d in $dupDoor) { $p += "duplicate door $($d.Name)" }
Check 3 'doors: on a wall cell, between two different rooms' $p

$p = @()
$seen = @{ 'Cafeteria' = $true }
$queue = [System.Collections.Queue]::new(); $queue.Enqueue('Cafeteria')
while ($queue.Count -gt 0) {
  $cur = $queue.Dequeue()
  foreach ($e in $edges) {
    foreach ($pair in @(@($e[0], $e[1]), @($e[1], $e[0]))) {
      if ($pair[0] -eq $cur -and -not $seen.ContainsKey($pair[1])) { $seen[$pair[1]] = $true; $queue.Enqueue($pair[1]) }
    }
  }
}
foreach ($r in $rooms) { if (-not $seen.ContainsKey($r.Name)) { $p += "$($r.Name) is not reachable from the Cafeteria" } }
Check 4 'connected: every room reachable from the Cafeteria' $p

$p = @()
foreach ($lang in 'English', 'French') {
  $keyed = Get-Content (Join-Path $root "Mod/Languages/$lang/Keyed/BetweenSus.xml") -Raw -Encoding UTF8
  foreach ($r in $rooms) { if ($keyed -notmatch "<BS_Room_$($r.Name)>[^<]+</BS_Room_$($r.Name)>") { $p += "$lang Keyed: no text for BS_Room_$($r.Name)" } }
}
Check 5 'keys: BS_Room_<Name> present and not empty in English and French' $p

if ($failed -eq 0) { Write-Host 'PASS: 5 of 5 checks' } else { Write-Host "FAIL: $failed of 5 checks" }
exit $failed
