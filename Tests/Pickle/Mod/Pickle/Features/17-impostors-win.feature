# S23 on BS_BetweenSus_TestSabotage (no AI kill): the AI impostor keeps sabotaging and nobody repairs, so a reactor or O2 sabotage runs out. Asserts the "[Between Sus] end: impostors" log line.
@requires:nelim.pickletools
@requires:nelim.pickletools.newcolony
@requires:nelim.pickletools.loadaudit
Feature: Between Sus impostors win

  Scenario: an unrepaired sabotage ends the game
    Given the main menu is open
    And Nelim's Pickle Tools: the new colony's seed is "betweensus-forced"
    And Nelim's Pickle Tools: the new colony's map size is 120
    And Nelim's Pickle Tools: the new colony's scenario is "BS_BetweenSus_TestSabotage"
    When Nelim's Pickle Tools: a new colony is started
    And Nelim's Pickle Tools: the new colony's colonists have landed
    And Nelim's Pickle Tools: the game is advanced by 100000 ticks
    Then Nelim's Pickle Tools: the game log holds the text "[Between Sus] end: impostors"
    And no errors were logged
