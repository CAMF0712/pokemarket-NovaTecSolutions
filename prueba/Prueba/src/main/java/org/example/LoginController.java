package org.example;

import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.fxml.FXMLLoader;
import javafx.scene.Scene;
import javafx.scene.control.*;
import javafx.stage.Stage;

public class LoginController {

    @FXML
    private TextField usernameField;

    @FXML
    private PasswordField passwordField;

    private final UserService userService = new UserService();

    @FXML
    public void loginUser(ActionEvent event) {

        String username = usernameField.getText();
        String password = passwordField.getText();

        if (!userService.userExists(username)) {

            showAlert("User not found",
                    "The user does not exist.\nPlease create an account.");

            return;
        }

        boolean valid = userService.validateLogin(username, password);

        if (valid) {

            showAlert("Login Successful",
                    "Welcome " + username);

            openWindow("add-card-view.fxml", event);

        } else {

            showAlert("Invalid Password",
                    "Incorrect password.");
        }
    }

    @FXML
    public void openRegister(ActionEvent event) {

        openWindow("register-view.fxml", event);
    }

    private void openWindow(String file, ActionEvent event) {

        try {

            FXMLLoader loader =
                    new FXMLLoader(FxApp.class.getResource("/org/example/" + file));

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