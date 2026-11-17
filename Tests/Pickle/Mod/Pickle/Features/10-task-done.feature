# S05 on the test scenario: a crewmate walks to a task station and completes a task, the message with the count appears.
# Covers the message and the absence of errors. "Station unavailable for good" and the 300 tick duration are not asserted.
@requires:nelim.pickletools
@requires:nelim.pickletools.newcolony
@requires:nelim.pickletools.loadaudit
Feature: Between Sus task completion

  Scenario: a crewmate completes a task
    Given the main menu is open
    And Nelim's Pickle Tools: the new colony's seed is "betweensus-forced"
    And Nelim's Pickle Tools: the new colony's map size is 120
    And Nelim's Pickle Tools: the new colony's scenario is "BS_BetweenSus_Test"
    When Nelim's Pickle Tools: a new colony is started
    And Nelim's Pickle Tools: the new colony's colonists have landed
    And Nelim's Pickle Tools: the game is advanced by 5000 ticks
    Then Nelim's Pickle Tools: a message "BS_TaskProgress" appeared
    And no errors were logged
