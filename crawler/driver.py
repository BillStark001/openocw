from selenium import webdriver
import time

'''
This scraper USES Selenium.
In principle, you can use requests, urllib, etc.
Some OCW pages use cookies and js to identify users and update pages.
So you have to get the page dynamically. "Selenium".
Selenium requires a browser driver.
Specifically https://www.selenium.dev/selenium/docs/api/py/api.html referring to.
'''

class DriverWrapper:
  
  def __init__(self,
               driver_path='chromedriver.exe', 
               mem_reset_limit=300, 
               html_timeout=60):
    
    self.mem_reset_limit = mem_reset_limit
    self.driver_path = driver_path
    self.html_timeout = html_timeout
    
    self.mem_reset_count = 0
    self.driver = None
    
  def is_initialized(self) -> bool:
    return self.driver is not None
    
  def init_driver(self):
    if self.driver != None:
      self.driver.quit()
      
    options = webdriver.ChromeOptions()
    #options.add_argument('user-agent="Mozilla/5.0 (Linux; Android 4.0.4; Galaxy Nexus Build/IMM76B) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.133 Mobile Safari/535.19"')
    options.add_argument('user_agent="Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.101 Safari/537.36"')
    options.add_argument('-Referer="http://www.ocw.titech.ac.jp/"')
    self.driver = webdriver.Chrome(self.driver_path, options=options)
    
    self.mem_reset_count = 0
    
  def get_html(self, *args, **kwargs) -> str:
    if self.mem_reset_count >= self.mem_reset_limit:
      self.init_driver()
      
    self.driver.get(*args, **kwargs)
    html = self.driver.page_source
    self.mem_reset_count += 1
    return html

  def get_html_after_loaded(self, *args, **kwargs) -> str:
    timeout = self.html_timeout
    
    html = self.get_html(*args, **kwargs)
    full_loaded = -1
    time_start = time.time()
    dtime = time.time() - time_start
    
    while full_loaded < 0:
      html = self.driver.page_source
      full_loaded = min(html.find('left-menu'), html.find('right-contents'))
      dtime = time.time() - time_start
      if timeout < dtime:
        return 'timeout'
      if html.find('HTTP 404') >= 0 or html.find('404 NOT FOUND') >= 0 or html.find('お探しのコンテンツが見つかりませんでした') >= 0:
        return '404'
      elif html.find('top-mein-navi') >= 0:
        return 'toppage'
    return html 
