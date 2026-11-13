param([Parameter(Mandatory)][string]$ModRoot, [string]$SharedDir)

# Own values of this mod that no shared check owns, offline, no game started. One consumer, nothing shareable: this mod's ship and rules.
# Distribution files and About link are Check-ModBasics; Harmony patch targets are Check-HarmonyContract; keys are Check-Translations.
# Tests/Test-Mod.ps1 (generated, after the prepublication) runs it after the shared checks. Prints findings, exit 1 on any.
#   1 about     packageId, hard dependency on Harmony, load after Harmony
#   2 roles     11 RoleDefs: 5 crewmates, 6 impostors, the two plain roles weigh 3
#   3 constants the numbers TESTING.md states (cooldowns, evidence, sabotage durations, ship size)
#   4 scenario  8 starting colonists, 2 impostors
#   5 props     one emergency button, one security and one admin console, stations of the furnisher
#   6 rooms     17 rooms, apart, 21 doors between two rooms, whole ship connected, BS_Room_* keys in English and French
$ErrorActionPreference = 'Stop'
$root = (Resolve-Path -LiteralPath $ModRoot).Path
$findings = @()
function Read-Text($rel) { Get-Content -LiteralPath (Join-Path $root $rel) -Raw -Encoding UTF8 }

try {
    # 1 about
    $about = ([xml](Read-Text 'Mod/About/About.xml')).ModMetaData
    if ($about.packageId -cne 'nelim.betweensus') { $findings += "packageId is $($about.packageId)" }
    if ($about.modDependencies.li.packageId -notcontains 'brrainz.harmony') { $findings += 'Missing hard dependency brrainz.harmony' }
    if ($about.loadAfter.li -notcontains 'brrainz.harmony') { $findings += 'Not loaded after brrainz.harmony' }

    # 2 roles
    $roles = ([xml](Read-Text 'Mod/Defs/RoleDefs.xml')).Defs.ChildNodes | Where-Object { $_.NodeType -eq 'Element' }
    $crew = @($roles | Where-Object { $_.team -eq 'Crewmate' }); $imp = @($roles | Where-Object { $_.team -eq 'Impostor' })
    if ($roles.Count -ne 11 -or $crew.Count -ne 5 -or $imp.Count -ne 6) { $findings += "roles: expected 11 (5 crewmates, 6 impostors), found $($roles.Count) ($($crew.Count), $($imp.Count))" }
    foreach ($r in $roles) {
        $want = if ($r.defName -in 'BS_Crewmate_Innocent', 'BS_Impostor_Impostor') { '3' } else { '' }
        if ("$($r.weight)" -ne $want) { $findings += "role $($r.defName): weight '$($r.weight)', expected '$want' (empty = default 1)" }
        if (-not $r.tip -or @($r.abilities.li).Count -lt 1) { $findings += "role $($r.defName): no tip or no ability" }
    }

    # 3 constants
    $ship = Read-Text 'Source/GameComponent_Ship.cs'
    foreach ($c in @(@('KillCooldown', 15000), @('SabotageCooldown', 30000), @('MeetingCooldown', 8000), @('EvidenceNeeded', 3))) {
        if ($ship -notmatch "const int $($c[0]) = $($c[1]);") { $findings += "constant $($c[0]) is no longer $($c[1])" }
    }
    foreach ($s in @('sabotageEnd = Now + 1800;', 'kind == SabotageKind.Reactor ? 5400 : 7200', 'sabotageEnd += 1800;', 'meetingCooldownEnd = Now + 2500;',
                     'killCooldownEnd[p] = Now + 12000;', 'KillCooldown * 0.5f', 'KillCooldown * 0.25f', 'vanishUntil[pawn] = Now + 900;', 'Now + 60000')) {
        if (-not $ship.Contains($s)) { $findings += "GameComponent_Ship.cs no longer holds: $s" }
    }
    $layout = Read-Text 'Source/ShipLayout.cs'
    if ($layout -notmatch 'Width = 95' -or $layout -notmatch 'Height = 47') { $findings += 'ship layout is no longer 95 x 47' }

    # 4 scenario
    $scen = Read-Text 'Mod/Defs/Scenario/BetweenSusScenario.xml'
    if ($scen -notmatch '<pawnCount>8</pawnCount>') { $findings += 'scenario: pawnCount is not 8' }
    if ($scen -notmatch '<impostorCount>2</impostorCount>') { $findings += 'scenario: impostorCount is not 2' }

    # 5 props
    $furn = Read-Text 'Source/ShipFurnisher.cs'
    foreach ($p in @(@('EmergencyButton', 1), @('SecurityConsole', 1), @('AdminConsole', 1), @('FixO2Station', 2), @('FixReactorStation', 1), @('TaskStation', 18))) {
        $n = ([regex]::Matches($furn, "Place\(`"BS_$($p[0])`"")).Count
        if ($n -ne $p[1]) { $findings += "ShipFurnisher: $($p[0]) placed $n times, expected $($p[1])" }
    }
    if ($furn -notmatch 'case "ReactorRear":\s*case "ReactorFront":') { $findings += 'ShipFurnisher: the two reactor rooms no longer share one case (one repair station each)' }

    # 6 rooms
    $rooms = @([regex]::Matches($layout, 'new Room\("(\w+)",\s*(\d+),\s*(\d+),\s*(\d+),\s*(\d+)\)') | ForEach-Object {
        [pscustomobject]@{ Name = $_.Groups[1].Value; X0 = [int]$_.Groups[2].Value; Z0 = [int]$_.Groups[3].Value; X1 = [int]$_.Groups[4].Value; Z1 = [int]$_.Groups[5].Value } })
    $doors = @([regex]::Matches($layout, 'new IntVec2\((\d+),\s*(\d+)\)') | ForEach-Object { [pscustomobject]@{ X = [int]$_.Groups[1].Value; Z = [int]$_.Groups[2].Value } })
    function RoomAt($x, $z) { $rooms | Where-Object { $x -ge $_.X0 -and $x -le $_.X1 -and $z -ge $_.Z0 -and $z -le $_.Z1 } | Select-Object -First 1 }

    if ($rooms.Count -ne 17) { $findings += "expected 17 rooms, found $($rooms.Count)" }
    if ($doors.Count -ne 21) { $findings += "expected 21 doors, found $($doors.Count)" }
    foreach ($d in $rooms | Group-Object Name | Where-Object Count -gt 1) { $findings += "duplicate room name $($d.Name)" }
    foreach ($r in $rooms) {
        if ($r.X0 -gt $r.X1 -or $r.Z0 -gt $r.Z1) { $findings += "$($r.Name): empty rectangle" }
        if ($r.X0 -lt 0 -or $r.Z0 -lt 0 -or $r.X1 -ge 95 -or $r.Z1 -ge 47) { $findings += "$($r.Name): outside the layout" }
    }
    for ($i = 0; $i -lt $rooms.Count; $i++) {
        for ($j = $i + 1; $j -lt $rooms.Count; $j++) {
            $a = $rooms[$i]; $b = $rooms[$j]
            $gapX = [Math]::Max($b.X0 - $a.X1, $a.X0 - $b.X1) - 1
            $gapZ = [Math]::Max($b.Z0 - $a.Z1, $a.Z0 - $b.Z1) - 1
            if ([Math]::Max($gapX, $gapZ) -lt 1) { $findings += "$($a.Name) and $($b.Name) overlap or touch (no wall cell between)" }
        }
    }
    $edges = @()
    foreach ($d in $doors) {
        $label = "door ($($d.X), $($d.Z))"
        if (RoomAt $d.X $d.Z) { $findings += "$label is inside a room"; continue }
        $w = RoomAt ($d.X - 1) $d.Z; $e = RoomAt ($d.X + 1) $d.Z; $s = RoomAt $d.X ($d.Z - 1); $n = RoomAt $d.X ($d.Z + 1)
        if ($w -and $e -and $w.Name -ne $e.Name) { $edges += , @($w.Name, $e.Name) }
        elseif ($s -and $n -and $s.Name -ne $n.Name) { $edges += , @($s.Name, $n.Name) }
        else { $findings += "$label has no two different rooms on opposite sides" }
    }
    foreach ($d in $doors | Group-Object { "$($_.X),$($_.Z)" } | Where-Object Count -gt 1) { $findings += "duplicate door $($d.Name)" }
    $seen = @{ 'Cafeteria' = $true }; $queue = [System.Collections.Queue]::new(); $queue.Enqueue('Cafeteria')
    while ($queue.Count -gt 0) {
        $cur = $queue.Dequeue()
        foreach ($e in $edges) {
            foreach ($pair in @(@($e[0], $e[1]), @($e[1], $e[0]))) {
                if ($pair[0] -eq $cur -and -not $seen.ContainsKey($pair[1])) { $seen[$pair[1]] = $true; $queue.Enqueue($pair[1]) }
            }
        }
    }
    foreach ($r in $rooms) { if (-not $seen.ContainsKey($r.Name)) { $findings += "$($r.Name) is not reachable from the Cafeteria" } }
    foreach ($lang in 'English', 'French') {
        $keyed = Read-Text "Mod/Languages/$lang/Keyed/BetweenSus.xml"
        foreach ($r in $rooms) { if ($keyed -notmatch "<BS_Room_$($r.Name)>[^<]+</BS_Room_$($r.Name)>") { $findings += "$lang Keyed: no text for BS_Room_$($r.Name)" } }
    }
} catch { $findings += "threw: $($_.Exception.GetBaseException().Message)" }

foreach ($f in $findings) { Write-Host $f }
if ($findings.Count) { exit 1 }
Write-Host 'Between Sus own values passed: about, 11 roles, constants, scenario, props, 17 rooms and 21 doors connected, room keys in English and French'
