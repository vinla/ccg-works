<template>
    <div class="bg-grey-lightest border border-black text-grey-darkest px-6 py-3 rounded relative" style="margin-top: 6rem; height: 10rem;" role="alert">    
        <div>
            <p class="font-bold py-2">{{data.prompt}}</p>
            <input v-model="value"
                :placeholder="placeholder"
                class="bg-white appearance-none
                border-2 border-grey-darker rounded
                w-full py-2 px-4 text-grey-darker leading-tight
                focus:outline-none" type="text">
            <div class="border-t pt-4 flex justify-end">
                <button class="bg-green-dark px-4 py-2 rounded text-white" type="button" @click="$emit('accept', {value})">{{acceptText}}</button>
                <button class="bg-red focus:outline-none text-white py-2 px-4 ml-2 rounded" @click="$emit('reject')" type="button">Cancel</button>
            </div>
        </div>
    </div>
</template>

<script>
export default {
    props: ['data'],
    computed: {
        placeholder: function() {
            if(this.data.hasOwnProperty('placeholder')) {
                return this.data.placeholder;
            }

            return '';
        },
        acceptText: function() {
            if(this.data.hasOwnProperty('acceptText')) {
                return this.data.acceptText;
            }

            return 'Ok';
        }
    },
    data: function() {
        return {
            value: ''
        }
    }    
}
</script>
