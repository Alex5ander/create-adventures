mergeInto(LibraryManager.library, {
  LoadData: function () {
    console.log('Load Data');
    let data = localStorage.getItem('SaveGame');

    if (!data) {
      return null;
    }

    let len = lengthBytesUTF8(data) + 1;
    let buffer = _malloc(len);
    stringToUTF8(data, buffer, len);

    return buffer;
  },

  SaveData: function (json) {
    console.log('SaveData');
    //UTF8ToString
    localStorage.setItem('SaveGame', UTF8ToString(json));
  },

  IsMobile: function() {
    return Module.SystemInfo.mobile;
  }
});
