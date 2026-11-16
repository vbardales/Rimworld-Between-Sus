# Proof of the role hook: the test scenario BS_BetweenSus_Test forces the roles by colonist name order.
# Expected log line: roles with BS_Impostor_Ninja and BS_Crewmate_Necromancer present, main = first colonist by name.
@requires:nelim.pickletools
@requires:nelim.pickletools.newcolony
@requires:nelim.pickletools.loadaudit
Feature: Between Sus forced roles

  Scenario: the test scenario assigns the forced roles
    Given the main menu is open
    And Nelim's Pickle Tools: the new colony's seed is "betweensus-forced"
    And Nelim's Pickle Tools: the new colony's map size is 120
    And Nelim's Pickle Tools: the new colony's scenario is "BS_BetweenSus_Test"
    When Nelim's Pickle Tools: a new colony is started
    And Nelim's Pickle Tools: the new colony's colonists have landed
    Then Nelim's Pickle Tools: the game log holds the text "BS_Impostor_Ninja"
    And Nelim's Pickle Tools: the game log holds the text "BS_Crewmate_Necromancer"
    And no errors were logged
