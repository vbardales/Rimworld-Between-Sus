# Probe: does a fixed seed give the same roles twice? Run twice, compare the "[Between Sus] roles:" line of Player.log in both evidence folders.
# The new colony takes minutes: one ticket per run, with -Extra '-pickle-scenario-timeout=400'.
@requires:nelim.pickletools
@requires:nelim.pickletools.newcolony
@requires:nelim.pickletools.loadaudit
Feature: Between Sus roles under a fixed seed

  Scenario: the roles are logged after a new colony with a fixed seed
    Given the main menu is open
    And Nelim's Pickle Tools: the new colony's seed is "betweensus-roles"
    And Nelim's Pickle Tools: the new colony's map size is 120
    And Nelim's Pickle Tools: the new colony's scenario is "BS_BetweenSus"
    When Nelim's Pickle Tools: a new colony is started
    And Nelim's Pickle Tools: the new colony's colonists have landed
    Then Nelim's Pickle Tools: the game log holds the text "[Between Sus] roles: main="
    And no errors were logged
