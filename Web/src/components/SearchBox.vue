<template>
  <div class="relative">
    <input type="text"
      class="transition focus:outline-0 border border-transparent focus:bg-white focus:border-grey-light placeholder-grey-darkest rounded bg-grey-lighter py-2 pr-4 pl-10 block appearance-none leading-normal ds-input w-full"
      :placeholder="placeholder" v-on:input="onSearch" aria-label="Search for games" />
    <div class="pointer-events-none absolute pin-y pin-l pl-3 flex items-center">
      <svg class="fill-current pointer-events-none text-grey-dark w-4 h-4" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20"><path d="M12.9 14.32a8 8 0 1 1 1.41-1.41l5.35 5.33-1.42 1.42-5.33-5.34zM8 14A6 6 0 1 0 8 2a6 6 0 0 0 0 12z"></path></svg>
    </div>
  </div>
</template>

<script>
export default {
  props: ["onSearch", "placeholder"]
};
</script>

