# S13 (b) and S15 on BS_BetweenSus_Test (main pawn = impostor, can vote). The button stands at (54, 78) (ship origin 13, 37 on a 120 map). The game is advanced past the 2500 tick lock first; noAutoButton stops random meetings, the first AI kill comes after 12000 ticks. Ticket 255a RED: "the screen is clear" drops every window Pickle does not own, Window_Meeting included, so windows are allowed to open again before the gizmo click. NOT PLAYED: steps `the field of the window` and `I call` are new in NPT (commit 56ac6b4e); `I click the button keyed "BS_Vote" number 1` hits the first Vote button of the window.
@requires:nelim.pickletools
@requires:nelim.pickletools.newcolony
@requires:nelim.pickletools.loadaudit
@requires:nelim.pickletools.keyedclick
@requires:nelim.pickletools.clickdiagnostics
Feature: Between Sus meeting and vote

  Scenario: a vote click opens the argument page and back returns to the list
    Given the main menu is open
    And Nelim's Pickle Tools: the new colony's seed is "betweensus-forced"
    And Nelim's Pickle Tools: the new colony's map size is 120
    And Nelim's Pickle Tools: the new colony's scenario is "BS_BetweenSus_Test"
    When Nelim's Pickle Tools: a new colony is started
    And Nelim's Pickle Tools: the new colony's colonists have landed
    And Nelim's Pickle Tools: the screen is clear
    And Nelim's Pickle Tools: the game is advanced by 3000 ticks
    And Nelim's Pickle Tools: windows are allowed to open again
    And Nelim's Pickle Tools: I select the thing of def "BS_EmergencyButton" at (54, 78)
    And Nelim's Pickle Tools: I click the gizmo keyed "BS_CallMeeting"
    Then Nelim's Pickle Tools: the window "Window_Meeting" is open
    Then Nelim's Pickle Tools: the button keyed "BS_Vote" number 1 is reachable in "Window_Meeting"
    When Nelim's Pickle Tools: the button keyed "BS_Vote" number 1 has stood still
    And Nelim's Pickle Tools: I click the button keyed "BS_Vote" number 1
    And Nelim's Pickle Tools: I click button keyed "BS_Back"
    And no errors were logged

  Scenario: an argued vote is cast
    Given the main menu is open
    And Nelim's Pickle Tools: the new colony's seed is "betweensus-forced"
    And Nelim's Pickle Tools: the new colony's map size is 120
    And Nelim's Pickle Tools: the new colony's scenario is "BS_BetweenSus_Test"
    When Nelim's Pickle Tools: a new colony is started
    And Nelim's Pickle Tools: the new colony's colonists have landed
    And Nelim's Pickle Tools: the screen is clear
    And Nelim's Pickle Tools: the game is advanced by 3000 ticks
    And Nelim's Pickle Tools: windows are allowed to open again
    And Nelim's Pickle Tools: I select the thing of def "BS_EmergencyButton" at (54, 78)
    And Nelim's Pickle Tools: I click the gizmo keyed "BS_CallMeeting"
    Then Nelim's Pickle Tools: the window "Window_Meeting" is open
    Then Nelim's Pickle Tools: the button keyed "BS_Vote" number 1 is reachable in "Window_Meeting"
    When Nelim's Pickle Tools: the button keyed "BS_Vote" number 1 has stood still
    And Nelim's Pickle Tools: I click the button keyed "BS_Vote" number 1
    And Nelim's Pickle Tools: the field "argument" of the window "Window_Meeting" is set to "he was near the body"
    And Nelim's Pickle Tools: I click button keyed "BS_ConfirmVote"
    Then Nelim's Pickle Tools: a message "BS_YouArgue" appeared
    And Nelim's Pickle Tools: the game log holds the text "meeting:"
    And no errors were logged

  Scenario: skipping closes the meeting with no player vote
    Given the main menu is open
    And Nelim's Pickle Tools: the new colony's seed is "betweensus-forced"
    And Nelim's Pickle Tools: the new colony's map size is 120
    And Nelim's Pickle Tools: the new colony's scenario is "BS_BetweenSus_Test"
    When Nelim's Pickle Tools: a new colony is started
    And Nelim's Pickle Tools: the new colony's colonists have landed
    And Nelim's Pickle Tools: the screen is clear
    And Nelim's Pickle Tools: the game is advanced by 3000 ticks
    And Nelim's Pickle Tools: windows are allowed to open again
    And Nelim's Pickle Tools: I select the thing of def "BS_EmergencyButton" at (54, 78)
    And Nelim's Pickle Tools: I click the gizmo keyed "BS_CallMeeting"
    Then Nelim's Pickle Tools: the window "Window_Meeting" is open
    When Nelim's Pickle Tools: I click button keyed "BS_Skip"
    And Nelim's Pickle Tools: the game is advanced by 300 ticks
    And no errors were logged
