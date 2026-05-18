package org.example;

import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.scene.control.*;
import javafx.stage.FileChooser;
import javafx.stage.Stage;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.nio.file.StandardCopyOption;
import java.io.File;

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

    private final CardService cardService = new CardService();

    private String selectedImagePath = "";

    @FXML
    public void initialize() {

        editionCombo.getItems().addAll(
                "1st Edition",
                "Unlimited"
        );

        languageCombo.getItems().addAll(
                "English",
                "Japanese",
                "Spanish"
        );

        finishCombo.getItems().addAll(
                "Holo",
                "Reverse Holo",
                "Non-Holo"
        );
    }

    @FXML
    public void addCard(ActionEvent event) {

        String cardName = cardNameField.getText();
        String set = setField.getText();
        String number = numberField.getText();

        String edition = editionCombo.getValue();
        String language = languageCombo.getValue();
        String finish = finishCombo.getValue();

        if (cardName.isEmpty()
                || set.isEmpty()
                || number.isEmpty()
                || edition == null
                || language == null
                || finish == null
                || selectedImagePath.isEmpty()) {

            showAlert(
                    "Error",
                    "Complete all fields."
            );

            return;
        }

        Card card = new Card(
                cardName,
                set,
                number,
                edition,
                language,
                finish,
                selectedImagePath
        );

        cardService.addCard(card);

        showAlert(
                "Success",
                "Card added successfully."
        );

        clearFields();
    }

    private void clearFields() {

        cardNameField.clear();
        setField.clear();
        numberField.clear();

        editionCombo.setValue(null);
        languageCombo.setValue(null);
        finishCombo.setValue(null);

        /*
         * Limpiar imagen seleccionada
         */
        selectedImagePath = "";
    }


    @FXML
    public void chooseImage(ActionEvent event) {

        FileChooser fileChooser = new FileChooser();

        fileChooser.setTitle("Select PNG Image");

        fileChooser.getExtensionFilters().add(
                new FileChooser.ExtensionFilter(
                        "PNG Files",
                        "*.png"
                )
        );

        Stage stage =
                (Stage)((Control)event.getSource())
                        .getScene()
                        .getWindow();

        File selectedFile =
                fileChooser.showOpenDialog(stage);

        if (selectedFile != null) {

            String fileName =
                    selectedFile.getName().toLowerCase();

            if (!fileName.endsWith(".png")) {

                showAlert(
                        "Error",
                        "Only PNG files are allowed."
                );

                selectedImagePath = "";

                return;
            }

            try {

                /*
                 * Carpeta destino dentro del proyecto
                 */
                String destinationFolder =
                        "prueba/Prueba/images/";

                /*
                 * Crear carpeta si no existe
                 */
                Files.createDirectories(
                        Paths.get(destinationFolder)
                );

                /*
                 * Ruta final
                 */
                Path destinationPath =
                        Paths.get(
                                destinationFolder
                                        + selectedFile.getName()
                        );

                /*
                 * Copiar imagen
                 */
                Files.copy(
                        selectedFile.toPath(),
                        destinationPath,
                        StandardCopyOption.REPLACE_EXISTING
                );

                /*
                 * Guardar ruta relativa
                 */
                selectedImagePath =
                        destinationPath.toString();

                showAlert(
                        "Success",
                        "Image saved in:\n"
                                + destinationPath
                );

            } catch (Exception e) {

                e.printStackTrace();

                showAlert(
                        "Error",
                        "Could not save image."
                );
            }
        }
    }

    private void showAlert(String title, String content) {

        Alert alert =
                new Alert(Alert.AlertType.INFORMATION);

        alert.setTitle(title);
        alert.setHeaderText(null);
        alert.setContentText(content);

        alert.showAndWait();
    }
}