<template>
  <on-click-outside :do="handleClickOutside">
    <div class="relative inline-block">
      <div role="button" class="inline-block select-none" @click="open = !open">
        <slot name="button"></slot>        
      </div>
      <div class="absolute pin-r mt-px z-40" v-show="open">
        <slot name="menu"></slot>
      </div>
    </div>
  </on-click-outside>
</template>

<script>
import OnClickOutside from "./OnClickOutside";

export default {
  props: ["text"],
  components: { OnClickOutside },
  methods: {
    handleClickOutside() {
      if (this.open) {
        this.open = false;
      }
    }
  },
  data: function() {
    return {
      open: false
    };
  }
};
</script>
