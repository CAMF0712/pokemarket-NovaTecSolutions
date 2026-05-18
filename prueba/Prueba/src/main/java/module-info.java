module org.example {
    requires javafx.controls;
    requires javafx.fxml;
    requires com.google.gson;

    exports org.example;
    opens org.example to javafx.fxml , com.google.gson;
}
