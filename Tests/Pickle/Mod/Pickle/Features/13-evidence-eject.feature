# S07 on the test scenario: enough evidence against an impostor lets the crew eliminate it without a vote.
# Asserts through the "[Between Sus] eject:" log line (GameComponent_Ship). The elimination is automatic (chance 0.5 every 250 ticks once an impostor holds 3 evidence), so wait 60000 ticks.
# The gizmo and the message for a crewmate main pawn are not asserted (window and message need NPT steps). One feature per goal.
@requires:nelim.pickletools
@requires:nelim.pickletools.newcolony
@requires:nelim.pickletools.loadaudit
Feature: Between Sus evidence elimination

  Scenario: three evidence against an impostor gets it eliminated
    Given the main menu is open
    And Nelim's Pickle Tools: the new colony's seed is "betweensus-forced"
    And Nelim's Pickle Tools: the new colony's map size is 120
    And Nelim's Pickle Tools: the new colony's scenario is "BS_BetweenSus_Test"
    When Nelim's Pickle Tools: a new colony is started
    And Nelim's Pickle Tools: the new colony's colonists have landed
    And Nelim's Pickle Tools: the game is advanced by 60000 ticks
    Then Nelim's Pickle Tools: the game log holds the text "[Between Sus] eject:"
    And no errors were logged
