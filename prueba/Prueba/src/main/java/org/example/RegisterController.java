package org.example;

import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.fxml.FXMLLoader;
import javafx.scene.Scene;
import javafx.scene.control.*;
import javafx.stage.Stage;

public class RegisterController {

    @FXML
    private TextField usernameField;

    @FXML
    private PasswordField passwordField;

    @FXML
    private PasswordField confirmPasswordField;

    private final UserService userService = new UserService();

    @FXML
    public void registerUser(ActionEvent event) {

        String username = usernameField.getText();
        String password = passwordField.getText();
        String confirmPassword = confirmPasswordField.getText();

        if (username.isEmpty()
                || password.isEmpty()
                || confirmPassword.isEmpty()) {

            showAlert("Error",
                    "Complete all fields.");

            return;
        }

        if (!password.equals(confirmPassword)) {

            showAlert("Error",
                    "Passwords do not match.");

            return;
        }

        if (userService.userExists(username)) {

            showAlert("Error",
                    "User already exists.");

            return;
        }

        userService.registerUser(username, password);

        showAlert("Success",
                "Account created successfully.");

        openLogin(event);
    }

    @FXML
    public void openLogin(ActionEvent event) {

        try {

            FXMLLoader loader =
                    new FXMLLoader(
                            FxApp.class.getResource(
                                    "/org/example/view.fxml"
                            )
                    );

            Scene scene = new Scene(loader.load());

            Stage stage =
                    (Stage)((Control)event.getSource())
                            .getScene()
                            .getWindow();

            stage.setScene(scene);
            stage.show();

        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    private void showAlert(String title, String content) {

        Alert alert = new Alert(Alert.AlertType.INFORMATION);

        alert.setTitle(title);
        alert.setHeaderText(null);
        alert.setContentText(content);

        alert.showAndWait();
    }
}