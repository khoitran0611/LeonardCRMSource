/**
 * Binds a TinyMCE widget to <textarea> elements.
 */
tfm_path = '/Scripts/tinyfilemanager';
angular.module('ui.tinymce', [])
  .value('uiTinymceConfig', {})
  .directive('uiTinymce', ['uiTinymceConfig', function (uiTinymceConfig) {
      uiTinymceConfig = uiTinymceConfig || {};
      var generatedIds = 0;
      return {
          //priority: 10,
          require: 'ngModel',
          link: function (scope, elm, attrs, ngModel) {
              var expression, options, tinyInstance,
                updateView = function (value) {
                    ngModel.$setViewValue(value);
                    if (!scope.$root.$$phase)  {
                        scope.$apply();
                    }
                };
              // generate an ID if not present
              if (!attrs.id) {
                  attrs.$set('id', 'uiTinymce' + generatedIds++);
              }
              if (attrs.uiTinymce) {
                  expression = scope.$eval(attrs.uiTinymce);
              } else {
                  expression = {};
              }
              options = {
                  // Update model when calling setContent (such as from the source editor popup)
                  setup: function (ed) {
                      var args;
                      ed.on('init', function (args) {
                          ngModel.$render();
                      });
                      // Update model on button click
                      ed.on('ExecCommand', function (e) {
                          ed.save();
                          updateView(elm.val());
                      });

                      // Update model on keypress
                      ed.on('KeyUp', function (e) {
                          ed.save();
                          updateView(elm.val());
                      });
                      // Update model on change, i.e. copy/pasted text, plugins altering content
                      ed.on('SetContent', function (e) {
                          if (!e.initial) {
                              if (elm.val()) {
                                  ed.save();
                                  updateView(elm.val());
                              }
                          }
                      });
                      if (expression.setup) {
                          scope.$eval(expression.setup);
                          delete expression.setup;
                      }
                  },
                  mode: 'exact',
                  elements: attrs.id,
                  plugins: [
                        'advlist autolink link image lists charmap print preview hr anchor pagebreak',
                        'searchreplace wordcount visualblocks visualchars code insertdatetime media nonbreaking',
                        'table contextmenu directionality emoticons template textcolor paste fullpage textcolor tinyfilemanager.net'
                  ],
                  toolbar: 'newdocument fullpage | bold italic underline strikethrough | alignleft aligncenter alignright alignjustify | styleselect fontselect fontsizeselect | cut copy paste | searchreplace | bullist numlist | outdent indent blockquote | undo redo | link unlink anchor image media code | inserttime preview | forecolor backcolor | hr removeformat | subscript superscript | charmap emoticons | print fullscreen | ltr rtl | spellchecker | visualchars visualblocks nonbreaking pagebreak restoredraft | table',
                  menubar: false,
                  toolbar_items_size: 'small',
                  style_formats: [
                        { title: 'Request Tour Button', inline: 'span', classes: 'LinkButton' },
                        { title: 'Towel', block: 'div', classes: 'towel' },
                        { title: 'Brown Text', inline: 'span', styles: { color: '#8a2e22' } }
                  ],
                  relative_urls: false,
                  remove_script_host: false,
                  convert_urls: false,
                  document_base_url: ""
              };
              // extend options with initial uiTinymceConfig and options from directive attribute value
              angular.extend(options, uiTinymceConfig, expression);
              setTimeout(function () {
                  tinymce.init(options);
              });

              ngModel.$render = function () {
                  if (!tinyInstance) {
                      tinyInstance = tinymce.get(attrs.id);
                  }
                  if (tinyInstance) {
                      if (ngModel.$viewValue) {
                          tinyInstance.setContent(ngModel.$viewValue || '');
                      }
                  }
              };

              scope.$on('$destroy', function () {
              	ensureInstance();

              	if (tinyInstance) {
              		tinyInstance.remove();
              		tinyInstance = null;
              	}
              });
				
              function ensureInstance() {
              	if (!tinyInstance) {
              		tinyInstance = tinymce.get(attrs.id);
              	}
              }
          }
      };
  }]);
