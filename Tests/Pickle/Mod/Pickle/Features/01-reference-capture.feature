# Reference captures of the crew in their suits, for the ghost art (layers drawn from a real colonist).
# Not an assertion suite: a green screenshot step says a file was captured, the owner looks at the pictures (@review).
# A new colony costs minutes and is not reproducible: play it once, as one small ticket.
@requires:nelim.pickletools
@requires:nelim.pickletools.newcolony
@requires:nelim.pickletools.camerazoom
Feature: Between Sus reference captures

  @review
  Scenario: the crew in their suits, wide then close
    Given the main menu is open
    And Nelim's Pickle Tools: the new colony's seed is "betweensus-ref"
    And Nelim's Pickle Tools: the new colony's map size is 120
    And Nelim's Pickle Tools: the new colony's scenario is "BS_BetweenSus"
    And Nelim's Pickle Tools: the screen is clear
    When Nelim's Pickle Tools: a new colony is started
    And Nelim's Pickle Tools: the new colony's colonists have landed
    And Nelim's Pickle Tools: time is paused for the capture
    And Nelim's Pickle Tools: I frame the cells (44, 69) to (65, 83) filling 65 percent of the screen
    And Nelim's Pickle Tools: the map is shown alone for a capture
    And I take a screenshot "betweensus-ship-cafeteria"
    And Nelim's Pickle Tools: I frame the cells (50, 73) to (58, 79) filling 30 percent of the screen
    And I take a screenshot "betweensus-crew-close"
    Then no errors were logged
