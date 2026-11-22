# S08 on BS_BetweenSus_TestSabotage (no AI kill): colonist 1 (Zed) is the main pawn and an impostor with the plain Impostor role (cooldown 15000), colonist 3 (Xia) is an Innocent crewmate. The first kill is ready 12000 ticks after the start. NOT PLAYED: step `stands next to` and `I pick the option` are new in NPT.
@requires:nelim.pickletools
@requires:nelim.pickletools.newcolony
@requires:nelim.pickletools.loadaudit
@requires:nelim.pickletools.colonistrace
@requires:nelim.pickletools.keyedclick
@requires:nelim.pickletools.inspecttabs
Feature: Between Sus kill gizmo

  Scenario: the blade is not ready at the start
    Given the main menu is open
    And Nelim's Pickle Tools: the new colony's seed is "betweensus-forced"
    And Nelim's Pickle Tools: the new colony's map size is 120
    And Nelim's Pickle Tools: the new colony's scenario is "BS_BetweenSus_TestSabotage"
    When Nelim's Pickle Tools: a new colony is started
    And Nelim's Pickle Tools: the new colony's colonists have landed
    And Nelim's Pickle Tools: the colonist number 1 is named "Zed"
    And Nelim's Pickle Tools: the colonist number 3 is named "Xia"
    And Nelim's Pickle Tools: "Xia" stands next to "Zed"
    And Nelim's Pickle Tools: I select the colonist named "Zed" alone
    And Nelim's Pickle Tools: I click the gizmo keyed "BS_Kill"
    Then Nelim's Pickle Tools: a message "BS_KillNotReady" appeared
    And no errors were logged

  Scenario: a ready kill hits the crewmate in reach, then the cooldown starts
    Given the main menu is open
    And Nelim's Pickle Tools: the new colony's seed is "betweensus-forced"
    And Nelim's Pickle Tools: the new colony's map size is 120
    And Nelim's Pickle Tools: the new colony's scenario is "BS_BetweenSus_TestSabotage"
    When Nelim's Pickle Tools: a new colony is started
    And Nelim's Pickle Tools: the new colony's colonists have landed
    And Nelim's Pickle Tools: the colonist number 1 is named "Zed"
    And Nelim's Pickle Tools: the colonist number 3 is named "Xia"
    And Nelim's Pickle Tools: the game is advanced by 12100 ticks
    And Nelim's Pickle Tools: "Xia" stands next to "Zed"
    And Nelim's Pickle Tools: I select the colonist named "Zed" alone
    And Nelim's Pickle Tools: I click the gizmo keyed "BS_Kill"
    Then Nelim's Pickle Tools: a message "BS_YouKilled" appeared
    And Nelim's Pickle Tools: the game log holds the text "[Between Sus] kill: Zed killed Xia"
    When Nelim's Pickle Tools: I click the gizmo keyed "BS_Kill"
    Then Nelim's Pickle Tools: a message "BS_KillNotReady" appeared
    And no errors were logged

  Scenario: nobody within reach
    Given the main menu is open
    And Nelim's Pickle Tools: the new colony's seed is "betweensus-forced"
    And Nelim's Pickle Tools: the new colony's map size is 120
    And Nelim's Pickle Tools: the new colony's scenario is "BS_BetweenSus_TestSabotage"
    When Nelim's Pickle Tools: a new colony is started
    And Nelim's Pickle Tools: the new colony's colonists have landed
    And Nelim's Pickle Tools: the colonist number 1 is named "Zed"
    And Nelim's Pickle Tools: the game is advanced by 12100 ticks
    And Nelim's Pickle Tools: "Zed" stands at (100, 100)
    And Nelim's Pickle Tools: I select the colonist named "Zed" alone
    And Nelim's Pickle Tools: I click the gizmo keyed "BS_Kill"
    Then Nelim's Pickle Tools: a message "BS_NoVictim" appeared
    And no errors were logged
