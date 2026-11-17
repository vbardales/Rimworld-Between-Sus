# S06 on the test scenario: an AI impostor fakes a task seen by a crewmate and gains evidence.
# Asserts through the "[Between Sus] evidence:" log line (AddEvidence). The first run (15000 ticks) saw no fake task: the AI impostor hunts first and the kill cooldown is 15000, so wait 40000.
# The 300 tick fake duration and the 2500 tick lockout are not asserted. One feature per goal.
@requires:nelim.pickletools
@requires:nelim.pickletools.newcolony
@requires:nelim.pickletools.loadaudit
Feature: Between Sus fake task evidence

  Scenario: a seen fake task gives evidence against an impostor
    Given the main menu is open
    And Nelim's Pickle Tools: the new colony's seed is "betweensus-forced"
    And Nelim's Pickle Tools: the new colony's map size is 120
    And Nelim's Pickle Tools: the new colony's scenario is "BS_BetweenSus_Test"
    When Nelim's Pickle Tools: a new colony is started
    And Nelim's Pickle Tools: the new colony's colonists have landed
    And Nelim's Pickle Tools: the game is advanced by 40000 ticks
    Then Nelim's Pickle Tools: the game log holds the text "[Between Sus] evidence:"
    And no errors were logged
