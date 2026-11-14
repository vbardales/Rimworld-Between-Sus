# Dye check of the candidate crew helmets worn with the chosen suit (Apparel_ArmorRecon): each helmet is worn by Cahuella, dyed, read back, and drawn in a portrait the owner looks at (@review).
# The first helmet (LisaWork welding mask) stayed silver whatever the dye (ticket 0b8c): a grey that stays grey is seen in the portrait.
# A new colony costs minutes and is not reproducible: play it once, as one small ticket.
@requires:nelim.pickletools
@requires:nelim.pickletools.newcolony
# Colonists of this seed: Cahuella, Dweeb, Dane, Hannibal, Misaki, Leon, Nails, Chaz (read from ticket edfd).
Feature: Between Sus helmet colours

  @review
  Scenario: the candidate helmets take a colour
    Given the main menu is open
    And Nelim's Pickle Tools: the new colony's seed is "betweensus-suits"
    And Nelim's Pickle Tools: the new colony's map size is 120
    And Nelim's Pickle Tools: the new colony's scenario is "BS_BetweenSus"
    And Nelim's Pickle Tools: the screen is clear
    When Nelim's Pickle Tools: a new colony is started
    And Nelim's Pickle Tools: the new colony's colonists have landed
    And Nelim's Pickle Tools: time is paused for the capture
    And Nelim's Pickle Tools: "Cahuella" wears "Apparel_ArmorRecon" dyed rgb (200, 40, 40)
    And Nelim's Pickle Tools: "Cahuella" wears "Apparel_VacsuitHelmet" dyed rgb (200, 40, 40)
    And Nelim's Pickle Tools: a portrait of "Cahuella" is saved as "helmet-VacsuitHelmet" at 400 pixels and zoom 2.0
    And Nelim's Pickle Tools: "Cahuella" wears "Apparel_ArmorHelmetRecon" dyed rgb (200, 40, 40)
    And Nelim's Pickle Tools: a portrait of "Cahuella" is saved as "helmet-ArmorHelmetRecon" at 400 pixels and zoom 2.0
    And Nelim's Pickle Tools: "Cahuella" wears "Apparel_ArmorHelmetReconPrestige" dyed rgb (200, 40, 40)
    And Nelim's Pickle Tools: a portrait of "Cahuella" is saved as "helmet-ArmorHelmetReconPrestige" at 400 pixels and zoom 2.0
    And Nelim's Pickle Tools: "Cahuella" wears "Apparel_GasMask" dyed rgb (200, 40, 40)
    And Nelim's Pickle Tools: a portrait of "Cahuella" is saved as "helmet-GasMask" at 400 pixels and zoom 2.0
    And Nelim's Pickle Tools: "Cahuella" wears "Apparel_PowerArmorHelmet" dyed rgb (200, 40, 40)
    And Nelim's Pickle Tools: a portrait of "Cahuella" is saved as "helmet-PowerArmorHelmet" at 400 pixels and zoom 2.0
    And Nelim's Pickle Tools: "Cahuella" wears "Apparel_AdvancedHelmet" dyed rgb (200, 40, 40)
    And Nelim's Pickle Tools: a portrait of "Cahuella" is saved as "helmet-AdvancedHelmet" at 400 pixels and zoom 2.0
    Then no errors were logged
