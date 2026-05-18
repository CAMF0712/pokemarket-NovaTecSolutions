package org.example;

import javafx.application.Application;
import javafx.fxml.FXMLLoader;
import javafx.scene.Scene;
import javafx.stage.Stage;

public class FxApp extends Application {
    @Override
    public void start(Stage stage) throws Exception {
        FXMLLoader fxmlLoader = new FXMLLoader(FxApp.class.getResource("/org/example/view.fxml"));
        Scene scene = new Scene(fxmlLoader.load());
        stage.setTitle("Prueba JavaFX");
        stage.setScene(scene);
        stage.show();
    }
}
