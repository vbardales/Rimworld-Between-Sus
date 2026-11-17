# S17 to S19 on the test scenario: an AI impostor starts a sabotage on its own (kind drawn at random, 15 percent chance per logic tick once the cooldown of 15000 ticks is over). Asserts the "[Between Sus] sabotage:" log line.
@requires:nelim.pickletools
@requires:nelim.pickletools.newcolony
@requires:nelim.pickletools.loadaudit
Feature: Between Sus AI sabotage

  Scenario: an AI impostor starts a sabotage
    Given the main menu is open
    And Nelim's Pickle Tools: the new colony's seed is "betweensus-forced"
    And Nelim's Pickle Tools: the new colony's map size is 120
    And Nelim's Pickle Tools: the new colony's scenario is "BS_BetweenSus_Test"
    When Nelim's Pickle Tools: a new colony is started
    And Nelim's Pickle Tools: the new colony's colonists have landed
    And Nelim's Pickle Tools: the game is advanced by 30000 ticks
    Then Nelim's Pickle Tools: the game log holds the text "[Between Sus] sabotage:"
    And no errors were logged
