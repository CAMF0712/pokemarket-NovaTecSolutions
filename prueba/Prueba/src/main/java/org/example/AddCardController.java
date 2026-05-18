package org.example;

import javafx.fxml.FXML;
import javafx.scene.control.ComboBox;
import javafx.scene.control.TextField;

public class AddCardController {

    @FXML
    private TextField cardNameField;

    @FXML
    private TextField setField;

    @FXML
    private TextField numberField;

    @FXML
    private ComboBox<String> editionCombo;

    @FXML
    private ComboBox<String> languageCombo;

    @FXML
    private ComboBox<String> finishCombo;

    @FXML
    public void initialize() {

        editionCombo.getItems().addAll(
                "First Edition",
                "Unlimited",
                "Shadowless"
        );

        languageCombo.getItems().addAll(
                "English",
                "Spanish",
                "Japanese"
        );

        finishCombo.getItems().addAll(
                "Holo",
                "Reverse Holo",
                "Full Art"
        );
    }

    @FXML
    public void addCard() {

        String cardName = cardNameField.getText();

        System.out.println("Adding card: " + cardName);
    }
}