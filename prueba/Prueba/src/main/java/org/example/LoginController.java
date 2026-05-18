package org.example;

import javafx.event.ActionEvent;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.stage.Stage;

public class LoginController {

    /*
     * LOGIN
     * For now it directly opens Add Card view
     */
    public void login(ActionEvent event) {

        try {

            FXMLLoader loader =
                    new FXMLLoader(
                            getClass().getResource("/org/example/add-card-view.fxml")
                    );

            Parent root = loader.load();

            Stage stage =
                    (Stage) ((javafx.scene.Node) event.getSource())
                            .getScene()
                            .getWindow();

            Scene scene = new Scene(root);

            scene.getStylesheets().add(
                    getClass()
                            .getResource("/org/example/style.css")
                            .toExternalForm()
            );

            stage.setTitle("PokéMarket - Add Card");

            stage.setScene(scene);

            stage.show();

        } catch (Exception e) {

            e.printStackTrace();

        }
    }

    /*
     * GO TO REGISTER VIEW
     */
    public void goToRegister(ActionEvent event) {

        try {

            FXMLLoader loader =
                    new FXMLLoader(
                            getClass().getResource("/org/example/register-view.fxml")
                    );

            Parent root = loader.load();

            Stage stage =
                    (Stage) ((javafx.scene.Node) event.getSource())
                            .getScene()
                            .getWindow();

            Scene scene = new Scene(root);

            scene.getStylesheets().add(
                    getClass()
                            .getResource("/org/example/style.css")
                            .toExternalForm()
            );

            stage.setTitle("PokéMarket - Register Account");

            stage.setScene(scene);

            stage.show();

        } catch (Exception e) {

            e.printStackTrace();

        }
    }
}