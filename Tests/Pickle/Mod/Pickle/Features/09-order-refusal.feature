# S04 on the test scenario: main pawn never refuses, impostor about 2 %, crewmate about 4 % at average mood and about 24 % at mood 0.
# 100 orders each, wide ranges (random draws). Mood step right before its orders step: the game pulls the mood back as ticks pass.
@requires:nelim.pickletools
@requires:nelim.pickletools.newcolony
@requires:nelim.pickletools.loadaudit
@requires:nelim.pickletools.colonistrace
Feature: Between Sus order refusal

  Scenario: refusal rates by role and mood
    Given the main menu is open
    And Nelim's Pickle Tools: the new colony's seed is "betweensus-forced"
    And Nelim's Pickle Tools: the new colony's map size is 120
    And Nelim's Pickle Tools: the new colony's scenario is "BS_BetweenSus_Test"
    When Nelim's Pickle Tools: a new colony is started
    And Nelim's Pickle Tools: the new colony's colonists have landed
    And Nelim's Pickle Tools: the colonist number 1 is named "Zed"
    And Nelim's Pickle Tools: the colonist number 2 is named "Yan"
    And Nelim's Pickle Tools: the colonist number 3 is named "Xia"
    And Nelim's Pickle Tools: "Zed" is ordered to go to (60, 60) 100 times
    Then Nelim's Pickle Tools: the orders of "Zed" were refused between 0 and 0 times
    When Nelim's Pickle Tools: "Yan" is ordered to go to (60, 60) 100 times
    Then Nelim's Pickle Tools: the orders of "Yan" were refused between 0 and 9 times
    When Nelim's Pickle Tools: "Xia" is ordered to go to (60, 60) 100 times
    Then Nelim's Pickle Tools: the orders of "Xia" were refused between 0 and 15 times
    Given Nelim's Pickle Tools: the mood of "Xia" is set to 0.0
    When Nelim's Pickle Tools: "Xia" is ordered to go to (60, 60) 100 times
    Then Nelim's Pickle Tools: the orders of "Xia" were refused between 10 and 40 times
    And no errors were logged
