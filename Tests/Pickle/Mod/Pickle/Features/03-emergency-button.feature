# Written at writeTests, NOT RUN: first run is at playTests (TESTING.md, scenario S13).
# The emergency button of the cafeteria is at layout cell (10, 9) of the cafeteria; with a map of 120 the ship origin is (13, 37), the cafeteria
# starts at (44, 69), so the button stands at (54, 78). The meeting button is refused during the first 2500 ticks (GameComponent_Ship.BeginGame).
# The screen is cleared after the landing so the two reveal windows do not hide the map.
@requires:nelim.pickletools
@requires:nelim.pickletools.newcolony
@requires:nelim.pickletools.loadaudit
Feature: Between Sus emergency button

  Scenario: the emergency button is refused at the start of the game
    Given the main menu is open
    And Nelim's Pickle Tools: the new colony's seed is "betweensus-button"
    And Nelim's Pickle Tools: the new colony's map size is 120
    And Nelim's Pickle Tools: the new colony's scenario is "BS_BetweenSus"
    When Nelim's Pickle Tools: a new colony is started
    And Nelim's Pickle Tools: the new colony's colonists have landed
    And Nelim's Pickle Tools: the screen is clear
    And Nelim's Pickle Tools: I select the thing of def "BS_EmergencyButton" at (54, 78)
    And Nelim's Pickle Tools: I click the gizmo keyed "BS_CallMeeting"
    Then Nelim's Pickle Tools: a message "BS_MeetingCooldown" appeared
    And no errors were logged
