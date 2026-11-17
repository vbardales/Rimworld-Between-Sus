# S14 on the test scenario: a kill leaves a body, a living crewmate who sees it opens a meeting. Asserts the "[Between Sus] meeting:" log line. A kill and a sighting are not guaranteed in the time given: wide wait.
@requires:nelim.pickletools
@requires:nelim.pickletools.newcolony
@requires:nelim.pickletools.loadaudit
Feature: Between Sus body report

  Scenario: an AI kill is followed by a body report meeting
    Given the main menu is open
    And Nelim's Pickle Tools: the new colony's seed is "betweensus-forced"
    And Nelim's Pickle Tools: the new colony's map size is 120
    And Nelim's Pickle Tools: the new colony's scenario is "BS_BetweenSus_Test"
    When Nelim's Pickle Tools: a new colony is started
    And Nelim's Pickle Tools: the new colony's colonists have landed
    And Nelim's Pickle Tools: the game is advanced by 60000 ticks
    Then Nelim's Pickle Tools: the game log holds the text "[Between Sus] meeting:"
    And no errors were logged
