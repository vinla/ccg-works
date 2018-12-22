import Vuex from 'vuex'
import Vue from 'vue'
import profileService from './services/profileService'

Vue.use(Vuex)

const state = {
    profile: {}
}

const getters = {
    isSignedIn: state => {
        return state.profile.hasOwnProperty('userName');
    }
}

const actions = {
    retrieveProfile ({commit}) {
        profileService.getProfile().then(p => commit('setProfile', p));
    }
}

const mutations = {
    setProfile (state, profile) {
        state.profile = profile;
    }
}

export default new Vuex.Store({
  state,
  getters,
  actions,
  mutations
})