import axios from "axios"

export default {
    saveDeck(deck) {
        return axios.post("deck", deck).then(r => r.data);
    },
    loadDecks(gameId) {
        return axios.get(`decks/${gameId}`).then(r => r.data);
    },
    deleteDeck(deckId) {
        return axios.delete(`deck/${deckId}`);
    }
}