# Written at writeTests, NOT RUN: first run is at playTests (TESTING.md, scenarios S01, S02, S27).
# The new colony takes minutes: one ticket, with -Extra '-pickle-scenario-timeout=400'.
@requires:nelim.pickletools
@requires:nelim.pickletools.newcolony
@requires:nelim.pickletools.loadaudit
Feature: Between Sus starts

  Scenario: a new colony starts on the ship, the team screen leads to the role screen, nothing is logged
    Given the main menu is open
    And Nelim's Pickle Tools: the new colony's seed is "betweensus-boot"
    And Nelim's Pickle Tools: the new colony's map size is 120
    And Nelim's Pickle Tools: the new colony's scenario is "BS_BetweenSus"
    When Nelim's Pickle Tools: a new colony is started
    And Nelim's Pickle Tools: the new colony's colonists have landed
    And Nelim's Pickle Tools: I click the button keyed "BS_Continue" and the window "Window_RoleReveal" opens
    Then Nelim's Pickle Tools: the load of the mod "nelim.betweensus" is clean
    And no errors were logged
