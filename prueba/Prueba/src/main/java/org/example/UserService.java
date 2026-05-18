package org.example;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;

import java.io.File;
import java.io.FileReader;
import java.io.FileWriter;
import java.lang.reflect.Type;
import java.util.ArrayList;
import java.util.List;

public class UserService {

    private static final String FILE_PATH = "users.json";

    private final Gson gson = new Gson();

    public List<User> loadUsers() {

        try {

            File file = new File(FILE_PATH);

            if (!file.exists()) {
                return new ArrayList<>();
            }

            FileReader reader = new FileReader(file);

            Type userListType = new TypeToken<List<User>>() {}.getType();

            List<User> users = gson.fromJson(reader, userListType);

            reader.close();

            if (users == null) {
                return new ArrayList<>();
            }

            return users;

        } catch (Exception e) {
            e.printStackTrace();
            return new ArrayList<>();
        }
    }

    public void saveUsers(List<User> users) {

        try {

            FileWriter writer = new FileWriter(FILE_PATH);

            gson.toJson(users, writer);

            writer.flush();
            writer.close();

        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public boolean userExists(String username) {

        List<User> users = loadUsers();

        for (User user : users) {

            if (user.getUsername().equals(username)) {
                return true;
            }
        }

        return false;
    }

    public boolean validateLogin(String username, String password) {

        List<User> users = loadUsers();

        for (User user : users) {

            if (user.getUsername().equals(username)
                    && user.getPassword().equals(password)) {

                return true;
            }
        }

        return false;
    }

    public void registerUser(String username, String password) {

        List<User> users = loadUsers();

        users.add(new User(username, password));

        saveUsers(users);
    }
}