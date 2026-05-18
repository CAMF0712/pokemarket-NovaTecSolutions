package org.example;

import javafx.event.ActionEvent;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.stage.Stage;

public class HomeController {

    /*
     * Open Register Account View
     */
    public void openRegisterView(ActionEvent event) {

        try {

            FXMLLoader loader =
                    new FXMLLoader(
                            getClass().getResource("/org/example/register-view.fxml")
                    );

            Parent root = loader.load();

            Stage stage = (Stage) ((javafx.scene.Node) event.getSource())
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

    /*
     * Open Add Card View
     */
    public void openAddCardView(ActionEvent event) {

        try {

            FXMLLoader loader =
                    new FXMLLoader(
                            getClass().getResource("/org/example/add-card-view.fxml")
                    );

            Parent root = loader.load();

            Stage stage = (Stage) ((javafx.scene.Node) event.getSource())
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
}