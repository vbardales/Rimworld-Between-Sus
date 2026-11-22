# S11 (part): the gizmos of the Scientist (Vitals) and of the Tracker (Track), on BS_BetweenSus_TestScientist and BS_BetweenSus_TestTracker (main pawn = colonist 5 and 6, no AI kill). Other roles (Engineer durations, Shapeshifter, Phantom, Necromancer, Acid) are not scripted here. NOT PLAYED: step `I pick the option` is new in NPT; the dialog window name `Dialog_MessageBox` is the game's class name.
@requires:nelim.pickletools
@requires:nelim.pickletools.newcolony
@requires:nelim.pickletools.loadaudit
@requires:nelim.pickletools.colonistrace
@requires:nelim.pickletools.keyedclick
@requires:nelim.pickletools.inspecttabs
@requires:nelim.pickletools.clearscreen
@requires:nelim.pickletools.clickdiagnostics
Feature: Between Sus role abilities

  Scenario: the Scientist Vitals gizmo lists the living colonists
    Given the main menu is open
    And Nelim's Pickle Tools: the new colony's seed is "betweensus-forced"
    And Nelim's Pickle Tools: the new colony's map size is 120
    And Nelim's Pickle Tools: the new colony's scenario is "BS_BetweenSus_TestScientist"
    When Nelim's Pickle Tools: a new colony is started
    And Nelim's Pickle Tools: the new colony's colonists have landed
    And Nelim's Pickle Tools: the colonist number 5 is named "Vera"
    And Nelim's Pickle Tools: the screen is clear
    And Nelim's Pickle Tools: windows are allowed to open again
    And Nelim's Pickle Tools: I select the colonist named "Vera" alone
    And Nelim's Pickle Tools: I click the gizmo keyed "BS_Vitals"
    Then Nelim's Pickle Tools: the window "Dialog_MessageBox" is open
    And no errors were logged

  Scenario: the Tracker Track gizmo starts a track
    Given the main menu is open
    And Nelim's Pickle Tools: the new colony's seed is "betweensus-forced"
    And Nelim's Pickle Tools: the new colony's map size is 120
    And Nelim's Pickle Tools: the new colony's scenario is "BS_BetweenSus_TestTracker"
    When Nelim's Pickle Tools: a new colony is started
    And Nelim's Pickle Tools: the new colony's colonists have landed
    And Nelim's Pickle Tools: the colonist number 1 is named "Zed"
    And Nelim's Pickle Tools: the colonist number 6 is named "Tara"
    And Nelim's Pickle Tools: I select the colonist named "Tara" alone
    And Nelim's Pickle Tools: I click the gizmo keyed "BS_TrackGizmo"
    And Nelim's Pickle Tools: I pick the option "Zed" of the open float menu
    Then Nelim's Pickle Tools: a message "BS_Tracking" appeared
    And no errors were logged
