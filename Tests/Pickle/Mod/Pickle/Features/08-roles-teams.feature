# S03 on the test scenario: 8 pawns, exactly 2 impostors, every other pawn a crewmate role, main pawn named in the roles line.
# Covers the team split only. The random draw and the weight 3 against 1 cannot be scripted (forced roles replace the draw).
@requires:nelim.pickletools
@requires:nelim.pickletools.newcolony
@requires:nelim.pickletools.loadaudit
Feature: Between Sus teams and roles

  Scenario: the test scenario gives 2 impostors and 6 crewmates
    Given the main menu is open
    And Nelim's Pickle Tools: the new colony's seed is "betweensus-forced"
    And Nelim's Pickle Tools: the new colony's map size is 120
    And Nelim's Pickle Tools: the new colony's scenario is "BS_BetweenSus_Test"
    When Nelim's Pickle Tools: a new colony is started
    And Nelim's Pickle Tools: the new colony's colonists have landed
    Then Nelim's Pickle Tools: the game log holds the text "[Between Sus] roles: main="
    And Nelim's Pickle Tools: the game log holds the text "=BS_Impostor_Impostor"
    And Nelim's Pickle Tools: the game log holds the text "=BS_Impostor_Ninja"
    And Nelim's Pickle Tools: the game log holds the text "=BS_Crewmate_Innocent"
    And Nelim's Pickle Tools: the game log holds the text "=BS_Crewmate_Engineer"
    And Nelim's Pickle Tools: the game log holds the text "=BS_Crewmate_Scientist"
    And Nelim's Pickle Tools: the game log holds the text "=BS_Crewmate_Tracker"
    And Nelim's Pickle Tools: the game log holds the text "=BS_Crewmate_Necromancer"
    And Nelim's Pickle Tools: the game log holds the text "=BS_Crewmate_Innocent"
    And no errors were logged
