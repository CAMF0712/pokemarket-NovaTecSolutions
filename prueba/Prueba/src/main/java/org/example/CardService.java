package org.example;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;

import java.io.File;
import java.io.FileReader;
import java.io.FileWriter;
import java.lang.reflect.Type;
import java.util.ArrayList;
import java.util.List;

public class CardService {

    private static final String FILE_PATH = "prueba/cards.json";

    private final Gson gson = new Gson();

    public List<Card> loadCards() {

        try {

            File file = new File(FILE_PATH);

            if (!file.exists()) {
                return new ArrayList<>();
            }

            FileReader reader = new FileReader(file);

            Type cardListType = new TypeToken<List<Card>>() {}.getType();

            List<Card> cards = gson.fromJson(reader, cardListType);

            reader.close();

            if (cards == null) {
                return new ArrayList<>();
            }

            return cards;

        } catch (Exception e) {
            e.printStackTrace();
            return new ArrayList<>();
        }
    }

    public void saveCards(List<Card> cards) {

        try {

            FileWriter writer = new FileWriter(FILE_PATH);

            gson.toJson(cards, writer);

            writer.flush();
            writer.close();

        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public void addCard(Card card) {

        List<Card> cards = loadCards();

        cards.add(card);

        saveCards(cards);
    }
}