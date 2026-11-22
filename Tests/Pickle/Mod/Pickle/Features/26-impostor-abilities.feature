# S11 (part): the gizmos of the Shapeshifter (Shapeshift) and of the Phantom (Vanish), on BS_BetweenSus_TestShapeshifter and BS_BetweenSus_TestPhantom (main pawn = colonist 1, no AI kill). Not scripted: colour restore after 60000 ticks, second use, witness effect of Vanish, Engineer durations, Necromancer, Acid. NOT PLAYED.
@requires:nelim.pickletools
@requires:nelim.pickletools.newcolony
@requires:nelim.pickletools.loadaudit
@requires:nelim.pickletools.colonistrace
@requires:nelim.pickletools.keyedclick
@requires:nelim.pickletools.inspecttabs
@requires:nelim.pickletools.clearscreen
@requires:nelim.pickletools.clickdiagnostics
Feature: Between Sus impostor abilities

  Scenario: the Shapeshift gizmo takes another colonist's colour
    Given the main menu is open
    And Nelim's Pickle Tools: the new colony's seed is "betweensus-forced"
    And Nelim's Pickle Tools: the new colony's map size is 120
    And Nelim's Pickle Tools: the new colony's scenario is "BS_BetweenSus_TestShapeshifter"
    When Nelim's Pickle Tools: a new colony is started
    And Nelim's Pickle Tools: the new colony's colonists have landed
    And Nelim's Pickle Tools: the colonist number 1 is named "Zed"
    And Nelim's Pickle Tools: I select the colonist named "Zed" alone
    And Nelim's Pickle Tools: I click the gizmo keyed "BS_ShapeshiftGizmo"
    Then Nelim's Pickle Tools: a message "BS_Shapeshifted" appeared
    And no errors were logged

  Scenario: the Vanish gizmo starts the fade
    Given the main menu is open
    And Nelim's Pickle Tools: the new colony's seed is "betweensus-forced"
    And Nelim's Pickle Tools: the new colony's map size is 120
    And Nelim's Pickle Tools: the new colony's scenario is "BS_BetweenSus_TestPhantom"
    When Nelim's Pickle Tools: a new colony is started
    And Nelim's Pickle Tools: the new colony's colonists have landed
    And Nelim's Pickle Tools: the colonist number 1 is named "Zed"
    And Nelim's Pickle Tools: I select the colonist named "Zed" alone
    And Nelim's Pickle Tools: I click the gizmo keyed "BS_VanishGizmo"
    Then Nelim's Pickle Tools: a message "BS_Vanished" appeared
    And no errors were logged
