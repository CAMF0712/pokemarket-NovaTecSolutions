package org.example;

public class Card {

    private String cardName;
    private String set;
    private String number;
    private String edition;
    private String language;
    private String finish;
    private String imagePath;

    public Card(String cardName,
                String set,
                String number,
                String edition,
                String language,
                String finish,
                String imagePath) {

        this.cardName = cardName;
        this.set = set;
        this.number = number;
        this.edition = edition;
        this.language = language;
        this.finish = finish;
        this.imagePath = imagePath;
    }

    public String getCardName() {
        return cardName;
    }

    public String getSet() {
        return set;
    }

    public String getNumber() {
        return number;
    }

    public String getEdition() {
        return edition;
    }

    public String getLanguage() {
        return language;
    }

    public String getFinish() {
        return finish;
    }

    public String getImagePath() {
        return imagePath;
    }
}