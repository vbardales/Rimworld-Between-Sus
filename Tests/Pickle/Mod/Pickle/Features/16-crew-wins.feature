# S22 on BS_BetweenSus_TestOneImpostor (one impostor holding 3 evidence at start): once it is eliminated the crew wins. Asserts the "[Between Sus] end: crew" log line.
@requires:nelim.pickletools
@requires:nelim.pickletools.newcolony
@requires:nelim.pickletools.loadaudit
Feature: Between Sus crew wins

  Scenario: eliminating the last impostor ends the game
    Given the main menu is open
    And Nelim's Pickle Tools: the new colony's seed is "betweensus-forced"
    And Nelim's Pickle Tools: the new colony's map size is 120
    And Nelim's Pickle Tools: the new colony's scenario is "BS_BetweenSus_TestOneImpostor"
    When Nelim's Pickle Tools: a new colony is started
    And Nelim's Pickle Tools: the new colony's colonists have landed
    And Nelim's Pickle Tools: the game is advanced by 20000 ticks
    Then Nelim's Pickle Tools: the game log holds the text "[Between Sus] end: crew"
    And no errors were logged
