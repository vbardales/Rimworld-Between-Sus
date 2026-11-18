# S21 on BS_BetweenSus_Test (ship origin 13, 37 on a 120 map): security console at (28, 65), admin console at (59, 63). The colonists land in the cafeteria: both reports name it.
@requires:nelim.pickletools
@requires:nelim.pickletools.newcolony
@requires:nelim.pickletools.loadaudit
@requires:nelim.pickletools.colonistrace
@requires:nelim.pickletools.keyedclick
@requires:nelim.pickletools.inspecttabs
Feature: Between Sus consoles

  Scenario: the security and admin consoles report the rooms
    Given the main menu is open
    And Nelim's Pickle Tools: the new colony's seed is "betweensus-forced"
    And Nelim's Pickle Tools: the new colony's map size is 120
    And Nelim's Pickle Tools: the new colony's scenario is "BS_BetweenSus_Test"
    When Nelim's Pickle Tools: a new colony is started
    And Nelim's Pickle Tools: the new colony's colonists have landed
    And Nelim's Pickle Tools: I select the thing of def "BS_SecurityConsole" at (28, 65)
    And Nelim's Pickle Tools: I click the gizmo keyed "BS_Cameras"
    Then Nelim's Pickle Tools: the window "Dialog_MessageBox" is open
    And Nelim's Pickle Tools: the window "Dialog_MessageBox" contains "Cafeteria"
    When Nelim's Pickle Tools: I close the window "Dialog_MessageBox"
    And Nelim's Pickle Tools: I select the thing of def "BS_AdminConsole" at (59, 63)
    And Nelim's Pickle Tools: I click the gizmo keyed "BS_ShipMap"
    Then Nelim's Pickle Tools: the window "Dialog_MessageBox" is open
    And Nelim's Pickle Tools: the window "Dialog_MessageBox" contains "Cafeteria"
    And no errors were logged
