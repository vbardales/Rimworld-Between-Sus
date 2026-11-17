# S10 on the test scenario: AI impostors hunt on their own. The main pawn is an impostor (forced), the other impostor is AI driven.
# Asserts through the log lines added to Kill, AddEvidence and Eject. A kill is not guaranteed in the time given: wide wait, one feature per goal.
@requires:nelim.pickletools
@requires:nelim.pickletools.newcolony
@requires:nelim.pickletools.loadaudit
Feature: Between Sus AI impostors

  Scenario: an AI impostor kills a crewmate on its own
    Given the main menu is open
    And Nelim's Pickle Tools: the new colony's seed is "betweensus-forced"
    And Nelim's Pickle Tools: the new colony's map size is 120
    And Nelim's Pickle Tools: the new colony's scenario is "BS_BetweenSus_Test"
    When Nelim's Pickle Tools: a new colony is started
    And Nelim's Pickle Tools: the new colony's colonists have landed
    And Nelim's Pickle Tools: the game is advanced by 40000 ticks
    Then Nelim's Pickle Tools: the game log holds the text "[Between Sus] kill:"
    And no errors were logged
