# S24 on BS_BetweenSus_TestEvidence: the main pawn (colonist 1, first impostor) holds 3 evidence and is ejected: it becomes a ghost. A ghost refuses an attack order (the refusal is a mote, so the step is "last order refused"), and walks. The emergency button is at (54, 78): ship origin 13, 37 plus cafeteria 31, 32 plus 10, 9.
@requires:nelim.pickletools
@requires:nelim.pickletools.newcolony
@requires:nelim.pickletools.loadaudit
@requires:nelim.pickletools.colonistrace
@requires:nelim.pickletools.keyedclick
@requires:nelim.pickletools.inspecttabs
Feature: Between Sus ghost orders

  Scenario: a ghost only does tasks and walks
    Given the main menu is open
    And Nelim's Pickle Tools: the new colony's seed is "betweensus-forced"
    And Nelim's Pickle Tools: the new colony's map size is 120
    And Nelim's Pickle Tools: the new colony's scenario is "BS_BetweenSus_TestEvidence"
    When Nelim's Pickle Tools: a new colony is started
    And Nelim's Pickle Tools: the new colony's colonists have landed
    And Nelim's Pickle Tools: the colonist number 1 is named "Zed"
    And Nelim's Pickle Tools: the game is advanced by 20000 ticks
    Then Nelim's Pickle Tools: the game log holds the text "[Between Sus] eject: Zed"
    Given Nelim's Pickle Tools: "Zed" is ordered to attack the thing "BS_EmergencyButton" at (54, 78)
    Then Nelim's Pickle Tools: the last order of "Zed" was refused
    When Nelim's Pickle Tools: "Zed" is ordered to go to (60, 60) 3 times
    Then Nelim's Pickle Tools: the orders of "Zed" were refused between 0 and 0 times
    And no errors were logged
