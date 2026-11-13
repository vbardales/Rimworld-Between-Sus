# Dye check of the candidate crew suits and of the helmet: each garment is worn by a colonist, dyed, read back, and drawn in a portrait the owner looks at (@review).
# Not an assertion suite beyond the dye read-back: a garment the game refuses to dye fails the step, a grey that stays grey is seen in the portrait.
# A new colony costs minutes and is not reproducible: play it once, as one small ticket.
@requires:nelim.pickletools
@requires:nelim.pickletools.newcolony
# Colonists of this seed: Cahuella, Dweeb, Dane, Hannibal, Misaki, Leon, Nails, Chaz (read from ticket edfd).
Feature: Between Sus suit colours

  @review
  Scenario: the candidate suits and the helmet take a colour
    Given the main menu is open
    And Nelim's Pickle Tools: the new colony's seed is "betweensus-suits"
    And Nelim's Pickle Tools: the new colony's map size is 120
    And Nelim's Pickle Tools: the new colony's scenario is "BS_BetweenSus"
    And Nelim's Pickle Tools: the screen is clear
    When Nelim's Pickle Tools: a new colony is started
    And Nelim's Pickle Tools: the new colony's colonists have landed
    And Nelim's Pickle Tools: time is paused for the capture
    And Nelim's Pickle Tools: "Cahuella" wears "Apparel_ArmorLocust" dyed rgb (200, 40, 40)
    And Nelim's Pickle Tools: a portrait of "Cahuella" is saved as "suit-ArmorLocust" at 400 pixels and zoom 1.0
    And Nelim's Pickle Tools: "Cahuella" wears "Apparel_ArmorRecon" dyed rgb (200, 40, 40)
    And Nelim's Pickle Tools: a portrait of "Cahuella" is saved as "suit-ArmorRecon" at 400 pixels and zoom 1.0
    And Nelim's Pickle Tools: "Cahuella" wears "Apparel_ArmorReconPrestige" dyed rgb (200, 40, 40)
    And Nelim's Pickle Tools: a portrait of "Cahuella" is saved as "suit-ArmorReconPrestige" at 400 pixels and zoom 1.0
    And Nelim's Pickle Tools: "Cahuella" wears "Apparel_ArmorMarineGrenadier" dyed rgb (200, 40, 40)
    And Nelim's Pickle Tools: a portrait of "Cahuella" is saved as "suit-ArmorMarineGrenadier" at 400 pixels and zoom 1.0
    And Nelim's Pickle Tools: "Cahuella" wears "Apparel_ArmorMarinePrestige" dyed rgb (200, 40, 40)
    And Nelim's Pickle Tools: a portrait of "Cahuella" is saved as "suit-ArmorMarinePrestige" at 400 pixels and zoom 1.0
    And Nelim's Pickle Tools: "Cahuella" wears "Apparel_PowerArmor" dyed rgb (200, 40, 40)
    And Nelim's Pickle Tools: a portrait of "Cahuella" is saved as "suit-PowerArmor" at 400 pixels and zoom 1.0
    And Nelim's Pickle Tools: "Cahuella" wears "Apparel_PlateArmor" dyed rgb (200, 40, 40)
    And Nelim's Pickle Tools: a portrait of "Cahuella" is saved as "suit-PlateArmor" at 400 pixels and zoom 1.0
    And Nelim's Pickle Tools: "Cahuella" wears "Apparel_ArmorCataphract" dyed rgb (200, 40, 40)
    And Nelim's Pickle Tools: a portrait of "Cahuella" is saved as "suit-ArmorCataphract" at 400 pixels and zoom 1.0
    And Nelim's Pickle Tools: "Cahuella" wears "Apparel_ArmorRecon" dyed rgb (200, 40, 40)
    And Nelim's Pickle Tools: "Cahuella" wears "LisaWork_ApparelWieldingMask" dyed rgb (200, 40, 40)
    And Nelim's Pickle Tools: a portrait of "Cahuella" is saved as "helmet-LisaWork_ApparelWieldingMask" at 400 pixels and zoom 2.0
    Then no errors were logged
