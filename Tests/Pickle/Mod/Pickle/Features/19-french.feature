# S26 (consoles part), played with -Language French: no raw BS_ key on any open window, the console reports and the role cards included.
@requires:nelim.pickletools
@requires:nelim.pickletools.newcolony
@requires:nelim.pickletools.loadaudit
@requires:nelim.pickletools.colonistrace
@requires:nelim.pickletools.keyedclick
@requires:nelim.pickletools.inspecttabs
Feature: Between Sus French

  Scenario: no raw key on screen in French
    Given the main menu is open
    And Nelim's Pickle Tools: the new colony's seed is "betweensus-forced"
    And Nelim's Pickle Tools: the new colony's map size is 120
    And Nelim's Pickle Tools: the new colony's scenario is "BS_BetweenSus_Test"
    When Nelim's Pickle Tools: a new colony is started
    And Nelim's Pickle Tools: the new colony's colonists have landed
    Then Nelim's Pickle Tools: no label on screen matches "BS_"
    When Nelim's Pickle Tools: I select the thing of def "BS_SecurityConsole" at (27, 65)
    And Nelim's Pickle Tools: I click the gizmo keyed "BS_Cameras"
    Then Nelim's Pickle Tools: the window "Dialog_MessageBox" contains "Cafétéria"
    And Nelim's Pickle Tools: no label on screen matches "BS_"
    When Nelim's Pickle Tools: I close the window "Dialog_MessageBox"
    And Nelim's Pickle Tools: I select the thing of def "BS_AdminConsole" at (59, 63)
    And Nelim's Pickle Tools: I click the gizmo keyed "BS_ShipMap"
    Then Nelim's Pickle Tools: no label on screen matches "BS_"
    And no errors were logged
